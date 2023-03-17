#include <Windows.h>
#include <fstream>

#define DEBUG
#ifdef DEBUG
#define LOG(x,z) {std::ofstream outfile(x,std::ios::app);outfile<<z<<std::endl;outfile.close();}
#else
#define LOG(x,z) 
#endif

static HINSTANCE g_hInstance = NULL;
static HHOOK g_hHook = NULL; // 钩子的句柄。

BOOL APIENTRY DllMain(HANDLE hDllHandle, DWORD dwReason, LPVOID lpreserved)
{
	g_hInstance = (HINSTANCE)hDllHandle;
	return TRUE;
}

/// <summary>与SetWindowsHookEx函数一起使用的应用程序定义或库定义的回调函数。</summary>
LRESULT CALLBACK KeyBoardProc(int code, WPARAM wParam, LPARAM lParam)
{	// 调用CallNextHookEx，把消息传给下一勾子。
	// 若要拦截消息，使消息不再传递下去，则返回1。
	// 若返回0，则表示本函数处理失败，系统会把消息传给下一个钩子，相当于调用CallNextHookEx。

	KBDLLHOOKSTRUCT* pKeyInfo = (KBDLLHOOKSTRUCT*)lParam;
	DWORD a = pKeyInfo->vkCode;

	// MSDN：flag的第8bit为按键按下抬起标志，按下为1，抬起为0。
	// 经测试，flag按下为0，抬起为128。按住不抬，则一直传入按下消息。
	LOG("KeyboardHook.log", "pKeyInfo->vkCode : " << pKeyInfo->vkCode);
	LOG("KeyboardHook.log", "pKeyInfo->scanCode : " << pKeyInfo->scanCode);
	LOG("KeyboardHook.log", "pKeyInfo->flags : " << pKeyInfo->flags);
	LOG("KeyboardHook.log", "pKeyInfo->time : " << pKeyInfo->time);

	if (code < 0)
	{ // 若 code < 0，则传递本消息。（钩子使用要求）
		return CallNextHookEx(g_hHook, code, wParam, lParam);
	}
	else
	{
		if ((pKeyInfo->flags & LLKHF_UP) == 1)
		{ // 所有按键，是否为抬起消息，是则不拦截。
			return CallNextHookEx(g_hHook, code, wParam, lParam);
		}
		else
		{ // 所有按键，是否为抬起消息，是则不拦截。
			/* 在WinUser.h中如下说明：
			 * VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
			 * Used only as parameters to GetAsyncKeyState() and GetKeyState().
			 * No other API or message will distinguish left and right keys in this way.
			 * 但是对Alt键的测试结果并非如此，在此低级键盘钩子的消息中，按左Alt，传入VK_LMENU。
			 */
			if ((a == VK_TAB) || (a == VK_RETURN) || (a == VK_CAPITAL) // 功能键：tab、enter、caps。
				|| (a >= VK_SPACE && a <= VK_HOME)  // 功能键：space，home，end，pgup，pgdn。
				|| (a == VK_INSERT) || (a == VK_DELETE) // 功能键。
				|| (a == VK_F5) || (a == VK_NUMLOCK) || (a == VK_BACK) // 功能键。
				|| (a == VK_CONTROL) || (a == VK_LCONTROL) || (a == VK_RCONTROL) // CONTROL。
				|| (a == VK_SHIFT) || (a == VK_LSHIFT) || (a == VK_RSHIFT) // SHIFT。
				|| (a >= VK_LEFT && a <= VK_DOWN) // 方向键上下左右。
				|| (a >= 0x30 && a <= 0x39) // 数字键0-9。
				|| (a >= 0x41 && a <= 0x5A) // 字母A-Z。
				|| (a >= VK_NUMPAD0 && a <= VK_DIVIDE) // 小键盘：0-9、乘、加、回车、减、点、除。
				|| (a >= VK_OEM_4 && a <= VK_OEM_7) || (a >= VK_OEM_1 && a <= VK_OEM_3) // 标点符号。
				)
			{ // 是否为允许的按键，不是则拦截。
				if ((pKeyInfo->flags & LLKHF_ALTDOWN) != 0) // alt状态会附带在其它键的按键消息中。
				{ // 按键信息中是否携带alt键，是则拦截。
					return 1;
				}
				else
				{ // 按键信息中是否携带alt键，是则拦截。
					// 使用VK_CONTROL来判断，因为其会在钩子传递之前就更新按键状态
					SHORT keyState = GetKeyState(VK_CONTROL);
					if (keyState < 0)
					{ // 是否ctrl键被按住，是则筛选按键。
						if (a == VK_SPACE // space。
							|| (a == VK_CONTROL) || (a == VK_LCONTROL) || (a == VK_RCONTROL) // CONTROL。
							|| (a == VK_SHIFT) || (a == VK_LSHIFT) || (a == VK_RSHIFT) // SHIFT。
							|| a == 0x43 || a == 0x58 || a == 0x56 // c、x、v。
							)
						{ // 是否按键为space、shift、ctrl、x、c、v，是则不拦截。
							return CallNextHookEx(g_hHook, code, wParam, lParam);
						}
						else
						{ // 是否按键为space、shift、ctrl、x、c、v，是则不拦截。
							return 1;
						}
					}
					else
					{ // 是否ctrl键被按住，是则筛选按键。
						return CallNextHookEx(g_hHook, code, wParam, lParam);
					}
				}
			}
			else
			{ // 是否为允许的按键，不是则拦截。
				return 1;
			}
		}
	}
}

extern "C"
{
	/// <summary>安装低级键盘钩子。
	/// <returns>
	/// <para>return FALSE：失败。</para>
	/// <para>return TRUE：成功。</para>
	/// </returns></summary>
	__declspec(dllexport) bool StartHook()
	{
		if (g_hHook)
		{
			return FALSE;
		}
		else
		{
			g_hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyBoardProc, g_hInstance, 0);
			if (g_hHook)
			{
				return TRUE;
			}
			else
			{
				return FALSE;
			}
		}
	}

	/// <summary>删除低级键盘钩子。
	/// <returns>
	/// <para>return FALSE：失败。</para>
	/// <para>return TRUE：成功。</para>
	/// </returns></summary>
	__declspec(dllexport) bool StopHook()
	{
		if (UnhookWindowsHookEx(g_hHook))
		{
			g_hHook = NULL;
			return TRUE;
		}
		else
		{
			return FALSE;
		}
	}
};