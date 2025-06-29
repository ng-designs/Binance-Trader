@echo off
echo 🚀 Запуск сборки Binance Trader на GitHub...
echo.

REM Проверяем наличие PowerShell
powershell -Command "Get-Host" >nul 2>&1
if errorlevel 1 (
    echo ❌ PowerShell не найден!
    pause
    exit /b 1
)

REM Запускаем PowerShell скрипт
powershell -ExecutionPolicy Bypass -File "scripts\build-and-monitor.ps1" %*

echo.
echo 🏁 Сборка завершена!
pause 