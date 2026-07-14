@echo off
setlocal enabledelayedexpansion

:: 配置项（根据实际项目修改）
set SERVICE_EXE=YS.Yuanji.Start.Service.exe
set SERVICE_NAME=QJYS_Serivce
set SERVICE_DISPLAY_NAME=QJYS_Serivce
set SERVICE_DESCRIPTION=终端数据采集服务.

:: 检查管理员权限
NET SESSION >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo 请求管理员权限...
    powershell -Command "Start-Process -FilePath '%~dpnx0' -Verb RunAs"
    exit /b
)

:: 进入脚本所在目录
cd /d "%~dp0"

:: 主菜单
echo 选择操作：
echo 1. 安装服务
echo 2. 卸载服务
echo 3. 启动服务
echo 4. 停止服务
echo 5. 退出
set /p choice=请输入数字 (1-5): 

if "%choice%"=="1" goto Install
if "%choice%"=="2" goto Uninstall
if "%choice%"=="3" goto Start
if "%choice%"=="4" goto Stop
if "%choice%"=="5" exit /b

:: 安装服务
:Install
echo 正在安装服务...
if exist "%SERVICE_EXE%" (
    :: 先尝试卸载旧服务（避免冲突）
    "%SERVICE_EXE%" uninstall >nul 2>&1

    :: 安装服务
    "%SERVICE_EXE%" install

    :: 配置服务描述和启动类型
    sc config "%SERVICE_NAME%" start=auto
    sc description "%SERVICE_NAME%" "%SERVICE_DESCRIPTION%"
    sc failure "%SERVICE_NAME%" reset=0 actions=restart/0

    :: 启动服务1
    net start "%SERVICE_NAME%"

    if %ERRORLEVEL% EQU 0 (
        echo 服务安装成功并已启动！
    ) else (
        echo 服务安装失败，请检查日志。
    )
) else (
    echo 错误: %SERVICE_EXE% 不存在！
)
goto End

:: 卸载服务
:Uninstall
echo 正在卸载服务...
if exist "%SERVICE_EXE%" (
    net stop "%SERVICE_NAME%" >nul 2>&1
    "%SERVICE_EXE%" uninstall
    sc delete "%SERVICE_NAME%" >nul 2>&1
    echo 服务已卸载
) else (
    echo 错误: %SERVICE_EXE% 不存在！
)
goto End

:: 启动服务
:Start
net start "%SERVICE_NAME%"
if %ERRORLEVEL% EQU 0 (
    echo 服务已启动
) else (
    echo 启动失败（可能已运行）
)
goto End

:: 停止服务
:Stop
net stop "%SERVICE_NAME%"
if %ERRORLEVEL% EQU 0 (
    echo 服务已停止
) else (
    echo 停止失败（可能未运行）
)
goto End

:End
pause