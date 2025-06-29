#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Скрипт для запуска GitHub Actions workflows

.DESCRIPTION
    Этот скрипт позволяет запускать различные GitHub Actions workflows:
    - Сборка проекта
    - Проверка качества кода
    - Обновление версии
    - Создание релиза

.PARAMETER Action
    Тип действия для выполнения:
    - build: Запуск сборки
    - quality: Запуск проверки качества кода
    - version: Обновление версии
    - release: Создание релиза

.PARAMETER VersionType
    Тип обновления версии (только для action=version):
    - major: Мажорная версия
    - minor: Минорная версия
    - patch: Патч версия

.PARAMETER CreateRelease
    Создать ли GitHub Release (только для action=version)

.PARAMETER Branch
    Ветка для запуска workflow (по умолчанию: main)

.PARAMETER Token
    GitHub Personal Access Token (если не указан, используется GITHUB_TOKEN)

.EXAMPLE
    .\trigger-github-actions.ps1 -Action build
    Запускает сборку проекта

.EXAMPLE
    .\trigger-github-actions.ps1 -Action version -VersionType patch -CreateRelease
    Обновляет патч версию и создает релиз

.EXAMPLE
    .\trigger-github-actions.ps1 -Action quality -Branch develop
    Запускает проверку качества кода в ветке develop
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("build", "quality", "version", "release")]
    [string]$Action,
    
    [Parameter(Mandatory = $false)]
    [ValidateSet("major", "minor", "patch")]
    [string]$VersionType = "patch",
    
    [Parameter(Mandatory = $false)]
    [switch]$CreateRelease,
    
    [Parameter(Mandatory = $false)]
    [string]$Branch = "main",
    
    [Parameter(Mandatory = $false)]
    [string]$Token
)

# Конфигурация
$RepoOwner = "HypsyNZ"
$RepoName = "BinanceTrader.NET"
$WorkflowFiles = @{
    "build" = "build.yml"
    "quality" = "code-quality.yml"
    "version" = "version-update.yml"
    "release" = "build.yml"
}

# Функция для получения GitHub Token
function Get-GitHubToken {
    if ($Token) {
        return $Token
    }
    
    $envToken = $env:GITHUB_TOKEN
    if ($envToken) {
        return $envToken
    }
    
    Write-Error "GitHub Token не найден. Укажите параметр -Token или установите переменную окружения GITHUB_TOKEN"
    exit 1
}

# Функция для получения текущего репозитория
function Get-CurrentRepository {
    try {
        $remoteUrl = git config --get remote.origin.url
        if ($remoteUrl -match "github\.com[:/]([^/]+)/([^/]+?)(?:\.git)?$") {
            $RepoOwner = $matches[1]
            $RepoName = $matches[2]
            Write-Host "Обнаружен репозиторий: $RepoOwner/$RepoName" -ForegroundColor Green
            return @{ Owner = $RepoOwner; Name = $RepoName }
        }
    }
    catch {
        Write-Warning "Не удалось определить репозиторий из git config"
    }
    
    return @{ Owner = $RepoOwner; Name = $RepoName }
}

# Функция для запуска workflow
function Start-GitHubWorkflow {
    param(
        [string]$WorkflowFile,
        [hashtable]$Inputs = @{}
    )
    
    $token = Get-GitHubToken
    $repo = Get-CurrentRepository
    
    $headers = @{
        "Authorization" = "token $token"
        "Accept" = "application/vnd.github.v3+json"
        "User-Agent" = "BinanceTrader-Build-Script"
    }
    
    $body = @{
        ref = $Branch
    }
    
    if ($Inputs.Count -gt 0) {
        $body.inputs = $Inputs
    }
    
    $jsonBody = $body | ConvertTo-Json -Depth 10
    
    $url = "https://api.github.com/repos/$($repo.Owner)/$($repo.Name)/actions/workflows/$WorkflowFile/dispatches"
    
    Write-Host "Запуск workflow: $WorkflowFile" -ForegroundColor Yellow
    Write-Host "URL: $url" -ForegroundColor Gray
    Write-Host "Body: $jsonBody" -ForegroundColor Gray
    
    try {
        $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $jsonBody -ContentType "application/json"
        Write-Host "✅ Workflow успешно запущен!" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Error "❌ Ошибка при запуске workflow: $($_.Exception.Message)"
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Error "Ответ сервера: $responseBody"
        }
        return $false
    }
}

# Функция для получения статуса последнего workflow
function Get-WorkflowStatus {
    param(
        [string]$WorkflowFile
    )
    
    $token = Get-GitHubToken
    $repo = Get-CurrentRepository
    
    $headers = @{
        "Authorization" = "token $token"
        "Accept" = "application/vnd.github.v3+json"
    }
    
    $url = "https://api.github.com/repos/$($repo.Owner)/$($repo.Name)/actions/workflows/$WorkflowFile/runs?per_page=1"
    
    try {
        $response = Invoke-RestMethod -Uri $url -Method Get -Headers $headers
        if ($response.workflow_runs.Count -gt 0) {
            $run = $response.workflow_runs[0]
            return @{
                Status = $run.status
                Conclusion = $run.conclusion
                RunId = $run.id
                Url = $run.html_url
            }
        }
    }
    catch {
        Write-Warning "Не удалось получить статус workflow: $($_.Exception.Message)"
    }
    
    return $null
}

# Функция для создания тега и релиза
function New-GitTagAndRelease {
    param(
        [string]$Version
    )
    
    Write-Host "Создание тега v$Version..." -ForegroundColor Yellow
    
    try {
        # Создание тега
        git tag -a "v$Version" -m "Release version $Version"
        git push origin "v$Version"
        
        Write-Host "✅ Тег v$Version создан и отправлен в репозиторий" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Error "❌ Ошибка при создании тега: $($_.Exception.Message)"
        return $false
    }
}

# Основная логика
Write-Host "🚀 Запуск GitHub Actions для Binance Trader" -ForegroundColor Cyan
Write-Host "Действие: $Action" -ForegroundColor White
Write-Host "Ветка: $Branch" -ForegroundColor White

switch ($Action) {
    "build" {
        $success = Start-GitHubWorkflow -WorkflowFile $WorkflowFiles.build
        if ($success) {
            Write-Host "📋 Ссылка на workflow: https://github.com/$RepoOwner/$RepoName/actions" -ForegroundColor Blue
        }
    }
    
    "quality" {
        $success = Start-GitHubWorkflow -WorkflowFile $WorkflowFiles.quality
        if ($success) {
            Write-Host "📋 Ссылка на workflow: https://github.com/$RepoOwner/$RepoName/actions" -ForegroundColor Blue
        }
    }
    
    "version" {
        $inputs = @{
            version_type = $VersionType
            create_release = $CreateRelease.IsPresent
        }
        
        $success = Start-GitHubWorkflow -WorkflowFile $WorkflowFiles.version -Inputs $inputs
        if ($success) {
            Write-Host "📋 Ссылка на workflow: https://github.com/$RepoOwner/$RepoName/actions" -ForegroundColor Blue
        }
    }
    
    "release" {
        # Получаем текущую версию из csproj
        $csprojPath = "BinanceTrader\Binance Trader.csproj"
        if (Test-Path $csprojPath) {
            $content = Get-Content $csprojPath -Raw
            if ($content -match '<Version>([^<]+)</Version>') {
                $version = $matches[1]
                Write-Host "Текущая версия: $version" -ForegroundColor Yellow
                
                # Создаем тег
                if (New-GitTagAndRelease -Version $version) {
                    Write-Host "✅ Релиз будет создан автоматически через GitHub Actions" -ForegroundColor Green
                    Write-Host "📋 Ссылка на релизы: https://github.com/$RepoOwner/$RepoName/releases" -ForegroundColor Blue
                }
            }
            else {
                Write-Error "Не удалось определить версию из csproj файла"
            }
        }
        else {
            Write-Error "Файл проекта не найден: $csprojPath"
        }
    }
}

Write-Host "`n🎉 Скрипт завершен!" -ForegroundColor Cyan 