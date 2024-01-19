::cd Scripts
::call fgui_model.bat
::cd ../
Auto_AddScriptToPro.exe PGameLogic.csproj Scripts bat_config.json debug android
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\" (cd /d "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE") else (cd /d "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE")
devenv %~dp0PGameLogic.sln /ReBuild "Debug"
pause