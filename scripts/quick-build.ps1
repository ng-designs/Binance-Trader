#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Быстрый запуск сборки Binance Trader через GitHub Actions

.DESCRIPTION
    Простой скрипт для запуска сборки с предустановленными параметрами

.EXAMPLE
    .\quick-build.ps1
    Запускает сборку Release x64

.EXAMPLE
    .\quick-build.ps1 -Debug
    Запускает сборку Debug x64

.EXAMPLE
    .\quick-build.ps1 -Platform x86
    Запускает сборку Release x86
#>

param(
    [switch]$Debug,
    [ValidateSet("x64", "x86", "AnyCPU")]
    [string]$Platform = "x64",
    [string]$Branch = "main"
)

# Конфигурация
$RepoOwner = "HypsyNZ"
$RepoName = "BinanceTrader.NET"

# Определяем конфигурацию
$Configuration = if ($Debug) { "Debug" } else { "Release" }

Write-Host "🚀 Быстрый запуск сборки Binance Trader" -ForegroundColor Cyan
Write-Host "Конфигурация: $Configuration" -ForegroundColor White
Write-Host "Платформа: $Platform" -ForegroundColor White
Write-Host "Ветка: $Branch" -ForegroundColor White

# Получаем токен
$token = $env:GITHUB_TOKEN
if (-not $token) {
    Write-Error "❌ GITHUB_TOKEN не найден в переменных окружения"
    Write-Host "Установите переменную окружения GITHUB_TOKEN или передайте токен в параметре" -ForegroundColor Yellow
    exit 1
}

# Получаем информацию о репозитории
try {
    $remoteUrl = git config --get remote.origin.url
    if ($remoteUrl -match "github\.com[:/]([^/]+)/([^/]+?)(?:\.git)?$") {
        $RepoOwner = $matches[1]
        $RepoName = $matches[2]
        Write-Host "Репозиторий: $RepoOwner/$RepoName" -ForegroundColor Green
    }
}
catch {
    Write-Warning "Не удалось определить репозиторий из git config, используем значения по умолчанию"
}

# Запускаем сборку
$headers = @{
    "Authorization" = "token $token"
    "Accept" = "application/vnd.github.v3+json"
    "User-Agent" = "BinanceTrader-QuickBuild"
}

$body = @{
    ref = $Branch
    inputs = @{
        configuration = $Configuration
        platform = $Platform
        create_artifacts = $true
        run_tests = $false
        branch = $Branch
    }
}

$jsonBody = $body | ConvertTo-Json -Depth 10
$url = "https://api.github.com/repos/$RepoOwner/$RepoName/actions/workflows/manual-build.yml/dispatches"

Write-Host "`n📡 Отправка запроса на сборку..." -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $jsonBody -ContentType "application/json"
    Write-Host "✅ Сборка успешно запущена!" -ForegroundColor Green
    Write-Host "📋 Ссылка на Actions: https://github.com/$RepoOwner/$RepoName/actions" -ForegroundColor Blue
    Write-Host "📋 Ссылка на Manual Build: https://github.com/$RepoOwner/$RepoName/actions/workflows/manual-build.yml" -ForegroundColor Blue
}
catch {
    Write-Error "❌ Ошибка при запуске сборки: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Error "Ответ сервера: $responseBody"
    }
    exit 1
}

Write-Host "`n🎉 Готово! Сборка запущена в GitHub Actions" -ForegroundColor Cyan 