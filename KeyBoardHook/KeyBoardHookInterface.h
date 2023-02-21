#ifndef __KEYBOARDHOOK_INTERFACE__
#define __KEYBOARDHOOK_INTERFACE__
#endif
// DLL功能：
// 使用低级键盘钩子，来过滤按键信息。
// 部分系统热键，无法过滤，但能获得按键信息。比如：ctrl+alt+del、辅助键。
extern "C"
{
	bool StartHook();
	bool StopHook();
};

