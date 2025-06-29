@echo off
chcp 65001 >nul
echo 🚀 Запуск сборки Binance Trader на GitHub
echo.

REM Запрашиваем GitHub Token
set /p GITHUB_TOKEN="Введите ваш GitHub Token: "

REM Устанавливаем переменную окружения
set GITHUB_TOKEN=%GITHUB_TOKEN%

REM Запускаем PowerShell скрипт
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Bypass -Command "& { $env:GITHUB_TOKEN = '%GITHUB_TOKEN%'; & 'scripts\simple-build.ps1' }"

echo.
echo 🏁 Сборка завершена!
pause 