rd /s /q 霸屏浏览器
md 霸屏浏览器
copy release\*.exe 霸屏浏览器\
copy release\*.dll 霸屏浏览器\
del 霸屏浏览器\*.vshost.exe
xcopy /e 程序所需的其它文件\* 霸屏浏览器\
cls
@echo off 
explorer 霸屏浏览器\
echo 完成。
echo 已整理为霸屏浏览器文件夹。
echo 霸屏浏览器启动程序 ：cs_fxb_win_hook.exe。
echo 设置程序           ：set_ini.exe。
pause