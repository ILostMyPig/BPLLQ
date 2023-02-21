// 下列 ifdef 块是创建使从 DLL 导出更简单的
// 宏的标准方法。此 DLL 中的所有文件都是用命令行上定义的 DISABLE_THE_ACCESSIBILITY_SHORTCUT_KEYS_EXPORTS
// 符号编译的。在使用此 DLL 的
// 任何其他项目上不应定义此符号。这样，源文件中包含此文件的任何其他项目都会将
// DISABLE_THE_ACCESSIBILITY_SHORTCUT_KEYS_API 函数视为是从 DLL 导入的，而此 DLL 则将用此宏定义的
// 符号视为是被导出的。
#ifdef DISABLE_THE_ACCESSIBILITY_SHORTCUT_KEYS_EXPORTS
#define DISABLE_THE_ACCESSIBILITY_SHORTCUT_KEYS_API __declspec(dllexport)
#else
#define DISABLE_THE_ACCESSIBILITY_SHORTCUT_KEYS_API __declspec(dllimport)
#endif

// DLL功能
// 屏蔽系统的辅助键：
// 滞粘键（STICKYKEYS）：左shift连按5次。
// 切换键（TOGGLEKEYS）：shift按住5秒。
// 筛选键（FILTERKEYS）：num lock键按住5秒。
// 鼠标键（MOUSEKEYS）：alt+左shift+num lock键。
// 高对比度（HIGHCONTRAST）：alt+shift+print screen键。
extern "C" DISABLE_THE_ACCESSIBILITY_SHORTCUT_KEYS_API void StartDisASK();
extern "C" DISABLE_THE_ACCESSIBILITY_SHORTCUT_KEYS_API void StopDisASK();