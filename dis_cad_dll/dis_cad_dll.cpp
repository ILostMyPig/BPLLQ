// dis_cad_dll.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "dis_cad_dll.h"

long WinlogonPID;

typedef long ( NTAPI *_NtSuspendProcess )( IN HANDLE ProcessHandle );
typedef long ( NTAPI *_NtResumeProcess )( IN HANDLE ProcessHandle );

bool WCHAR_same(WCHAR *a, WCHAR *b, int c);
long GetWinlogonPID(void);

/// <summary>将当前进程提权。
/// <returns>
/// <para>return true：成功。</para>
/// <para>return false：失败。</para>
/// </returns></summary>
DIS_CAD_DLL_API bool EnableDebugPrivilege(void)
{
	DWORD e;
	WCHAR ss[11];
	LPVOID lpMsgBuf;

	bool re;
	TOKEN_PRIVILEGES TP;

	long r;
	//提升进程权限
	HANDLE gcp;

	gcp=GetCurrentProcess();
	e = GetLastError();
	FormatMessage (
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL,
		e,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR) &lpMsgBuf,
		0, NULL );
	swprintf(ss, 11, L"%s", lpMsgBuf);
	if(e != 0)MessageBox(NULL, ss, _T("EDP -> GetCurrentProcess"),MB_OK);

	HANDLE hToken;
	r = OpenProcessToken(gcp, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &hToken);
	e = GetLastError();
	FormatMessage (
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL,
		e,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR) &lpMsgBuf,
		0, NULL );
	swprintf(ss, 11, L"%s", lpMsgBuf);
	if(e != 0)MessageBox(NULL, ss, _T("EDP -> OpenProcessToken"),MB_OK);
	if (r &&  !e)
	{
		r = LookupPrivilegeValue(NULL, L"SeDebugPrivilege", &TP.Privileges[0].Luid);
		e = GetLastError();
		FormatMessage (
			FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
			NULL,
			e,
			MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
			(LPTSTR) &lpMsgBuf,
			0, NULL );
		swprintf(ss, 11, L"%s", lpMsgBuf);
		if(e != 0)MessageBox(NULL, ss, _T("EDP -> LookupPrivilegeValue"),MB_OK);
		if (r && !e)
		{
			TP.PrivilegeCount = 1;
			TP.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
			r = AdjustTokenPrivileges(hToken, false, &TP, sizeof(TP), 0, 0);
			e = GetLastError();
			FormatMessage (
				FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
				NULL,
				e,
				MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
				(LPTSTR) &lpMsgBuf,
				0, NULL );
			swprintf(ss, 11, L"%s", lpMsgBuf);
			if(e != 0)MessageBox(NULL, ss, _T("EDP -> AdjustTokenPrivileges"),MB_OK);
			if(e == 0)
			{
				re = true;
			}
			else
			{
				re=false;
			}
		}
		else
		{
			re = false;
		}
	}
	else
	{
		re = false;
	}

	CloseHandle(hToken);
	return re;
}

/// <summary>挂起winlogon进程。若多次调用本函数，也需调用同样多的 ResWin 函数才能恢复进程。
/// <returns>
/// <para>return ：成功。</para>
/// </returns></summary>
DIS_CAD_DLL_API bool SusWin(void)
{
	DWORD e;
	WCHAR ss[11];
	LPVOID lpMsgBuf;

	HANDLE hP;
	hP = OpenProcess(PROCESS_ALL_ACCESS, false, GetWinlogonPID());
	e = GetLastError();
	FormatMessage (
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL,
		e,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR) &lpMsgBuf,
		0, NULL );
	swprintf(ss, 11, L"%s", lpMsgBuf);
	if(e != 0)MessageBox(NULL, ss, _T("SW -> OpenProcess"),MB_OK);
	if (hP == 0 )
	{
		return false;
	}

	_NtSuspendProcess NtSuspendProcess = 0;
	NtSuspendProcess = (_NtSuspendProcess)
		GetProcAddress( GetModuleHandle( L"ntdll" ), "NtSuspendProcess" );

	long re;
	re=NtSuspendProcess(hP) >= 0;
	e = GetLastError();
	FormatMessage (
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL,
		e,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR) &lpMsgBuf,
		0, NULL );
	swprintf(ss, 11, L"%s", lpMsgBuf);
	if(e != 0)MessageBox(NULL, ss, _T("SW -> OpenProcess"),MB_OK);
	CloseHandle( hP);

	return true;
}

/// <summary>获取当前进程的PID。
/// <returns>
/// <para>return ：PID。</para>
/// </returns></summary>
long GetWinlogonPID(void)
{
	DWORD e;
	WCHAR ss[11];
	LPVOID lpMsgBuf;

	long re = 0;
	long lngResult;
	HANDLE hSnapShot;

	PROCESSENTRY32 PEE ;
	hSnapShot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	e = GetLastError();
	FormatMessage (
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL,
		e,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR) &lpMsgBuf,
		0, NULL );
	swprintf(ss, 11, L"%s", lpMsgBuf);
	if(e != 0)MessageBox(NULL, ss, _T("GWPID -> CreateToolhelp32Snapshot"),MB_OK);


	PEE.dwSize = sizeof(PEE);
	lngResult = Process32First(hSnapShot, &PEE);
	e = GetLastError();
	FormatMessage (
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL,
		e,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR) &lpMsgBuf,
		0, NULL );
	swprintf(ss, 11, L"%s", lpMsgBuf);
	if(e != 0)MessageBox(NULL, ss, _T("GWPID -> Process32First"),MB_OK);

	//建立进程快照，循环查找进程

	WCHAR strExe[13]={0};
	while(lngResult != 0)
	{

		for (int i =0;i<=11;i++)//赋值
		{
			strExe[i]=PEE.szExeFile[i];
		}

		bool x;
		WCHAR strsame[]=L"winlogon.exe";
		x = WCHAR_same(strExe, strsame,12);

		if( true == x) //找到winlogon.exe则返回
		{
			re= PEE.th32ProcessID;
			CloseHandle( hSnapShot);
			WinlogonPID=re;
			return re;
		}
		lngResult = Process32Next(hSnapShot, &PEE);
		e = GetLastError();
		FormatMessage (
			FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
			NULL,
			e,
			MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
			(LPTSTR) &lpMsgBuf,
			0, NULL );
		swprintf(ss, 11, L"%s", lpMsgBuf);
		if(e != 0)MessageBox(NULL, ss, _T("GWPID -> Process32Next"),MB_OK);
	}
	CloseHandle (hSnapShot);
	return re;
}

/// <summary>恢复winlogin进程。若多次调用 SusWin 函数，也需调用同样多的本函数才能恢复进程。
/// <returns>
/// <para>return true：成功。</para>
/// <para>return false：失败。</para>
/// </returns></summary>
DIS_CAD_DLL_API bool ResWin(void)
{
	HANDLE hP;
	hP = OpenProcess(PROCESS_ALL_ACCESS, false, WinlogonPID);
	if (hP == 0)
	{
		return false;
	}

	_NtResumeProcess NtResumeProcess = 0;
	NtResumeProcess = (_NtResumeProcess)
		GetProcAddress( GetModuleHandle( _T("ntdll") ), "NtResumeProcess" );

	bool re;
	re=NtResumeProcess(hP) >= 0;

	CloseHandle (hP);
	return re;
}

/// <summary>比较两个 WCHAR 类型的数据是否相同。
/// <returns>
/// <para>return false：不同。</para>
/// <para>return true：相同。</para>
/// </returns></summary>
/// <param name="a">第一个 WCHAR 数据。</param>
/// <param name="b">第二个 WCHAR 数据。</param>
/// <param name="c">WCHAR 中需要对比的元素数量。</param>
bool WCHAR_same(WCHAR *a, WCHAR *b, int c)
{
	for (int i=0;i<c;i++)
	{
		if( a[i] != b[i])
		{
			return false;
		}
	}
	return true;
}