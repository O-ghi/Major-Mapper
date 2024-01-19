set doPause=0
set doPause=%1%

::cd Scripts
::call fgui_model.bat
::cd ../
Auto_AddScriptToPro.exe PGameLogic.csproj Scripts bat_config.json release android
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\" (cd /d "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE") else (cd /d "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE")
devenv %~dp0PGameLogic.sln /ReBuild "Release"

if "%doPause%" == "" ( pause ) else ping 127.0.0.1 -n 5 >nul