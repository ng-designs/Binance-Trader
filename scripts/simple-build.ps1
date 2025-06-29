#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Простой запуск сборки на GitHub
#>

param(
    [switch]$Debug,
    [ValidateSet("x64", "x86", "AnyCPU")]
    [string]$Platform = "x64",
    [string]$Token
)

# Конфигурация
$RepoOwner = "HypsyNZ"
$RepoName = "BinanceTrader.NET"
$Configuration = if ($Debug) { "Debug" } else { "Release" }

Write-Host "🚀 Запуск сборки Binance Trader на GitHub" -ForegroundColor Cyan
Write-Host "Конфигурация: $Configuration" -ForegroundColor White
Write-Host "Платформа: $Platform" -ForegroundColor White

# Получаем токен
if (-not $Token) {
    $Token = $env:GITHUB_TOKEN
}

if (-not $Token) {
    Write-Error "❌ GitHub Token не найден!"
    Write-Host "Укажите параметр -Token или установите переменную окружения GITHUB_TOKEN" -ForegroundColor Yellow
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
    Write-Warning "Не удалось определить репозиторий из git config"
}

# Запускаем сборку
$headers = @{
    "Authorization" = "token $Token"
    "Accept" = "application/vnd.github.v3+json"
    "User-Agent" = "BinanceTrader-Build"
}

$body = @{
    ref = "main"
    inputs = @{
        configuration = $Configuration
        platform = $Platform
        create_artifacts = $true
        run_tests = $false
        branch = "main"
    }
}

$jsonBody = $body | ConvertTo-Json -Depth 10
$url = "https://api.github.com/repos/$RepoOwner/$RepoName/actions/workflows/manual-build.yml/dispatches"

Write-Host "`n📡 Запуск сборки..." -ForegroundColor Yellow

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