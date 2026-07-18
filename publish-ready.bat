@echo off
setlocal EnableExtensions EnableDelayedExpansion

cd /d "%~dp0"

echo ========================================
echo Jinchoge publish helper
echo ========================================
echo.

where dotnet >nul 2>nul
if errorlevel 1 (
    echo ERROR: .NET SDK was not found on PATH.
    echo Install the .NET 6 SDK from https://dotnet.microsoft.com/download/dotnet/6.0 and run this file again.
    echo.
    pause
    exit /b 1
)

for /f "tokens=*" %%v in ('dotnet --version 2^>nul') do set "DOTNET_VERSION=%%v"
echo Found .NET SDK version: %DOTNET_VERSION%
echo.

echo Cleaning previous failed publish outputs...
call :CleanPublishArtifacts
if errorlevel 1 goto :Fail

echo.
echo What do you want to publish?
echo   1^) GUI app  ^(Jinchoge.exe^) [recommended]
echo   2^) CLI app  ^(Jinchoge.CLI.exe^)
choice /c 12 /n /m "Choose 1 or 2: "
if errorlevel 2 (
    set "PROJECT=Yuyuyui.PrivateServer.CLI\Yuyuyui.PrivateServer.CLI.csproj"
    set "EXE_NAME=Jinchoge.CLI.exe"
    set "APP_NAME=CLI"
) else (
    set "PROJECT=Yuyuyui.PrivateServer.GUI\Yuyuyui.PrivateServer.GUI.csproj"
    set "EXE_NAME=Jinchoge.exe"
    set "APP_NAME=GUI"
)

echo.
echo Which Windows version do you want?
echo   1^) 64-bit Windows ^(win-x64^) [recommended]
echo   2^) 32-bit Windows ^(win-x86^)
choice /c 12 /n /m "Choose 1 or 2: "
if errorlevel 2 (
    set "RID=win-x86"
) else (
    set "RID=win-x64"
)

echo.
echo Bundle .NET runtime into the exe folder?
echo   Y^) Yes, ready to use on PCs without .NET installed [recommended]
echo   N^) No, smaller output but target PC needs .NET Desktop Runtime 6
choice /c YN /n /m "Choose Y or N: "
if errorlevel 2 (
    set "SELF_CONTAINED=false"
) else (
    set "SELF_CONTAINED=true"
)

set "PUBLISH_DIR=publish\%APP_NAME%-%RID%"

echo.
echo Publishing %APP_NAME% for %RID%...
echo Output folder: %PUBLISH_DIR%
echo.

dotnet restore YuyuyuiPrivateServer.sln
if errorlevel 1 goto :Fail

dotnet publish "%PROJECT%" -c Release -r %RID% --self-contained %SELF_CONTAINED% -p:PublishSingleFile=true -o "%PUBLISH_DIR%"
if errorlevel 1 goto :Fail

echo.
echo Cleaning intermediate build files after publish...
dotnet clean YuyuyuiPrivateServer.sln -c Release >nul
if exist "Yuyuyui.PrivateServer.CLI\obj" rmdir /s /q "Yuyuyui.PrivateServer.CLI\obj"
if exist "Yuyuyui.PrivateServer.GUI\obj" rmdir /s /q "Yuyuyui.PrivateServer.GUI\obj"
if exist "Yuyuyui.PrivateServer\obj" rmdir /s /q "Yuyuyui.PrivateServer\obj"
if exist "Yuyuyui.PrivateServer.DataModel\obj" rmdir /s /q "Yuyuyui.PrivateServer.DataModel\obj"
if exist "Yuyuyui.PrivateServer.Localization\obj" rmdir /s /q "Yuyuyui.PrivateServer.Localization\obj"
if exist "Yuyuyui.GoalKeeper\obj" rmdir /s /q "Yuyuyui.GoalKeeper\obj"

set "EXE_PATH=%PUBLISH_DIR%\%EXE_NAME%"
if not exist "%EXE_PATH%" (
    echo ERROR: Publish finished but %EXE_PATH% was not found.
    echo.
    pause
    exit /b 1
)

echo.
echo Publish complete: %EXE_PATH%
echo Opening the exe now...
start "" "%EXE_PATH%"
echo.
pause
exit /b 0

:CleanPublishArtifacts
if exist "publish" rmdir /s /q "publish"
if exist "Yuyuyui.PrivateServer.CLI\bin\Release" rmdir /s /q "Yuyuyui.PrivateServer.CLI\bin\Release"
if exist "Yuyuyui.PrivateServer.GUI\bin\Release" rmdir /s /q "Yuyuyui.PrivateServer.GUI\bin\Release"
if exist "Yuyuyui.PrivateServer\bin\Release" rmdir /s /q "Yuyuyui.PrivateServer\bin\Release"
if exist "Yuyuyui.PrivateServer.DataModel\bin\Release" rmdir /s /q "Yuyuyui.PrivateServer.DataModel\bin\Release"
if exist "Yuyuyui.PrivateServer.Localization\bin\Release" rmdir /s /q "Yuyuyui.PrivateServer.Localization\bin\Release"
if exist "Yuyuyui.GoalKeeper\bin\Release" rmdir /s /q "Yuyuyui.GoalKeeper\bin\Release"
dotnet clean YuyuyuiPrivateServer.sln -c Release >nul
exit /b 0

:Fail
echo.
echo ERROR: Publish failed. Check the messages above.
echo.
pause
exit /b 1
