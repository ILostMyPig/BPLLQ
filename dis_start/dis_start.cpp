// dis_start.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "dis_start.h"

HWND re_gdi = NULL; // 开始按钮的句柄。
HWND re_fw = NULL; // 开始栏的句柄。

/// <summary>隐藏开始按钮和开始栏。
/// <returns>
/// <para>return 0：成功。</para>
/// <para>return -1：没有找到开始栏的句柄。</para>
/// <para>return -2：没有找到开始按钮的句柄。</para>
/// </returns></summary>
DIS_START_API int StartdisStart(void)
{
	BOOL re;

	// 隐藏开始栏。
	re_fw=FindWindow(L"Shell_TrayWnd", NULL); // 获取开始栏的句柄。
	if(re_fw == NULL){return -1;}
	re = ShowWindow(re_fw, SW_HIDE); // 隐藏开始栏。

	// 隐藏开始按钮。
	re_gdi=FindWindow(L"Button", NULL); // 获取开始按钮的句柄。
	if(re_gdi == NULL){return -2;}
	re = ShowWindow(re_gdi, SW_HIDE); // 隐藏开始按钮。

	return 0;
}

/// <summary>显示开始按钮和开始栏。
/// <returns>
/// <para>return 0：成功。</para>
/// </returns></summary>
DIS_START_API int StopdisStart(void)
{
	BOOL re;

	re = ShowWindow(re_gdi, SW_SHOW); // 显示开始栏。
	re = ShowWindow(re_fw, SW_SHOW); // 显示开始按钮。

	return 0;
}