#include <Windows.h>
#include <fstream>

//#define DEBUG
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

bool IsKeyAllowed(DWORD vk)
{
    // 功能键：Tab、回车、CapsLock、Insert、Delete、F5、NumLock、Backspace
    if (vk == VK_TAB || vk == VK_RETURN || vk == VK_CAPITAL ||
        vk == VK_INSERT || vk == VK_DELETE || vk == VK_F5 ||
        vk == VK_NUMLOCK || vk == VK_BACK)
        return true;

    // 范围：Space (0x20) 到 Home (0x24) 之间的键（包括 Space、PageUp、PageDown、End、Home）
    if (vk >= VK_SPACE && vk <= VK_HOME)
        return true;

    // 方向键：Left (0x25) 到 Down (0x28)
    if (vk >= VK_LEFT && vk <= VK_DOWN)
        return true;

    // 数字键 0-9
    if (vk >= '0' && vk <= '9')
        return true;

    // 字母 A-Z
    if (vk >= 'A' && vk <= 'Z')
        return true;

    // 小键盘：0-9、乘、加、回车、减、点、除 (0x60-0x6F)
    if (vk >= VK_NUMPAD0 && vk <= VK_DIVIDE)
        return true;

    // 标点符号：OEM_1 到 OEM_3 (;: = ,< .> /?) 和 OEM_4 到 OEM_7 ([{ \| ]} '")
    if ((vk >= VK_OEM_1 && vk <= VK_OEM_3) ||
        (vk >= VK_OEM_4 && vk <= VK_OEM_7))
        return true;

    return false;
}

/// <summary>与SetWindowsHookEx函数一起使用的应用程序定义或库定义的回调函数。</summary>
LRESULT CALLBACK KeyBoardProc(int code, WPARAM wParam, LPARAM lParam)
{	// 调用CallNextHookEx，把消息传给下一勾子。
	// 若要拦截消息，使消息不再传递下去，则返回1。
	// 若返回0，则表示本函数处理失败，系统会把消息传给下一个钩子，相当于调用CallNextHookEx。

	KBDLLHOOKSTRUCT* pKeyInfo = (KBDLLHOOKSTRUCT*)lParam;
	DWORD vk = pKeyInfo->vkCode;

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
		if (pKeyInfo->flags & LLKHF_UP)
		{ // 所有按键，是否为抬起消息，是则不拦截。
			return CallNextHookEx(g_hHook, code, wParam, lParam);
		}
		else
		{
			/* 在WinUser.h中如下说明：
			 * VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
			 * Used only as parameters to GetAsyncKeyState() and GetKeyState().
			 * No other API or message will distinguish left and right keys in this way.
			 * 但是对Alt键的测试结果并非如此，在此低级键盘钩子的消息中，按左Alt，传入VK_LMENU。
			 */
						
			// 处理修饰键本身
			// Ctrl 或 Shift 允许按下
			if (vk == VK_CONTROL || vk == VK_LCONTROL || vk == VK_RCONTROL ||
				vk == VK_SHIFT || vk == VK_LSHIFT || vk == VK_RSHIFT)
			{
				return CallNextHookEx(g_hHook, code, wParam, lParam);
			}

			// Alt 或 Win 直接拦截
			if (vk == VK_MENU || vk == VK_LMENU || vk == VK_RMENU ||
				vk == VK_LWIN || vk == VK_RWIN)
			{
				return 1;
			}

			// 获取当前修饰键物理状态
			bool bCtrl = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
			bool bShift = (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0;
			bool bAlt = (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;
			bool bWin = (GetAsyncKeyState(VK_LWIN) & 0x8000) != 0 || (GetAsyncKeyState(VK_RWIN) & 0x8000) != 0;
			// 非修饰键按键
			bool hasModifier = bCtrl || bShift || bAlt || bWin;
			if (!hasModifier)
			{
				// 无修饰键：只允许列表中的按键
				if (IsKeyAllowed(vk))
					return CallNextHookEx(g_hHook, code, wParam, lParam);
				else
					return 1;
			}
			else
			{
				// 有修饰键：只允许 Ctrl+Shift（且无 Alt/Win）
				if (bCtrl && bShift && !bAlt && !bWin)
				{
					if (IsKeyAllowed(vk))
						return CallNextHookEx(g_hHook, code, wParam, lParam);
					else
						return 1;
				}
				else
				{
					// 其他所有组合键拦截
					return 1;
				}
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