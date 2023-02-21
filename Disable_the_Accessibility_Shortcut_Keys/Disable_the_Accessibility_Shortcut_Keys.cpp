// Disable_the_Accessibility_Shortcut_Keys.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "Disable_the_Accessibility_Shortcut_Keys.h"

// 存放 StartDisASK 函数中获取的当前配置。
STICKYKEYS g_StartupStickyKeys = {sizeof(STICKYKEYS), 0}; // 辅助键“滞粘键”的数据结构。
TOGGLEKEYS g_StartupToggleKeys = {sizeof(TOGGLEKEYS), 0}; // 辅助键“切换键”的数据结构。
FILTERKEYS g_StartupFilterKeys = {sizeof(FILTERKEYS), 0}; // 辅助键“筛选键”的数据结构。
MOUSEKEYS g_StartupMouseKeys = {sizeof(MOUSEKEYS), 0}; // 辅助键“鼠标键”的数据结构。
HIGHCONTRAST g_StartupHighContrast = {sizeof(HIGHCONTRAST), 0}; // 辅助键“高对比度”的数据结构。


/// <summary>启用辅助键。
/// </summary>
DISABLE_THE_ACCESSIBILITY_SHORTCUT_KEYS_API void StopDisASK()
{  
	BOOL re;
	// 使用 StartDisASK 函数中获取的当前配置来恢复。
	re = SystemParametersInfo(SPI_SETSTICKYKEYS, sizeof(STICKYKEYS), &g_StartupStickyKeys, 0);
	re = SystemParametersInfo(SPI_SETTOGGLEKEYS, sizeof(TOGGLEKEYS), &g_StartupToggleKeys, 0);
	re = SystemParametersInfo(SPI_SETFILTERKEYS, sizeof(FILTERKEYS), &g_StartupFilterKeys, 0);
	re = SystemParametersInfo(SPI_SETMOUSEKEYS, sizeof(MOUSEKEYS), &g_StartupMouseKeys, 0);
	re = SystemParametersInfo(SPI_SETHIGHCONTRAST, sizeof(HIGHCONTRAST), &g_StartupHighContrast, 0);
}

/// <summary>禁用辅助键。
/// </summary>
DISABLE_THE_ACCESSIBILITY_SHORTCUT_KEYS_API void StartDisASK()
{
	BOOL re;
	// 获取当前配置。
	re = SystemParametersInfo(SPI_GETSTICKYKEYS, sizeof(STICKYKEYS), &g_StartupStickyKeys, 0); 
	re = SystemParametersInfo(SPI_GETTOGGLEKEYS, sizeof(TOGGLEKEYS), &g_StartupToggleKeys, 0);
	re = SystemParametersInfo(SPI_GETFILTERKEYS, sizeof(FILTERKEYS), &g_StartupFilterKeys, 0);
	re = SystemParametersInfo(SPI_GETMOUSEKEYS, sizeof(MOUSEKEYS), &g_StartupMouseKeys, 0);
	re = SystemParametersInfo(SPI_GETHIGHCONTRAST, sizeof(HIGHCONTRAST), &g_StartupHighContrast, 0);

	// 将当前配置复制到存放新配置的变量中。
	STICKYKEYS skOff = g_StartupStickyKeys;
	TOGGLEKEYS tkOff = g_StartupToggleKeys;
	FILTERKEYS fkOff = g_StartupFilterKeys;
	MOUSEKEYS mkOff = g_StartupMouseKeys;
	HIGHCONTRAST hcOff = g_StartupHighContrast;

	// 将新配置的效果变为禁用辅助键，然后将新配置生效。
	skOff.dwFlags &= ~SKF_HOTKEYACTIVE;
	skOff.dwFlags &= ~SKF_CONFIRMHOTKEY;
	re = SystemParametersInfo(SPI_SETSTICKYKEYS, sizeof(STICKYKEYS), &skOff, 0);

	tkOff.dwFlags &= ~TKF_HOTKEYACTIVE;
	tkOff.dwFlags &= ~TKF_CONFIRMHOTKEY;
	re = SystemParametersInfo(SPI_SETTOGGLEKEYS, sizeof(TOGGLEKEYS), &tkOff, 0);

	fkOff.dwFlags &= ~FKF_HOTKEYACTIVE;
	fkOff.dwFlags &= ~FKF_CONFIRMHOTKEY;
	re = SystemParametersInfo(SPI_SETFILTERKEYS, sizeof(FILTERKEYS), &fkOff, 0);

	mkOff.dwFlags &= ~FKF_HOTKEYACTIVE;
	mkOff.dwFlags &= ~FKF_CONFIRMHOTKEY;
	re = SystemParametersInfo(SPI_SETMOUSEKEYS, sizeof(MOUSEKEYS), &mkOff, 0);

	hcOff.dwFlags &= ~FKF_HOTKEYACTIVE;
	hcOff.dwFlags &= ~FKF_CONFIRMHOTKEY;
	re = SystemParametersInfo(SPI_SETHIGHCONTRAST, sizeof(HIGHCONTRAST), &hcOff, 0);
}