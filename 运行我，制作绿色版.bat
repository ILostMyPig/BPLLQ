@echo on
"C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe" "%CD%\cs_fxb_win_hook.sln" /rebuild "release"
rd /s /q 霸屏浏览器
md 霸屏浏览器
copy release\*.exe 霸屏浏览器\
copy release\*.dll 霸屏浏览器\
del 霸屏浏览器\*.vshost.exe
xcopy /e 程序所需的其它文件\* 霸屏浏览器\
@echo off
echo.
echo - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
echo.
echo 注：如果安装vs2010时，修改了安装路径，则要手动生成程序后，再执行我。
echo.
echo 制作完成！
echo.
echo 霸屏浏览器启动程序 ：cs_fxb_win_hook.exe。
echo 设置程序 ：set_ini.exe。
echo.
echo - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
explorer 霸屏浏览器\
pause