#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Запуск сборки на GitHub с мониторингом в реальном времени

.DESCRIPTION
    Запускает сборку через GitHub Actions и показывает логи в реальном времени

.EXAMPLE
    .\build-and-monitor.ps1
    Запускает сборку Release x64 и показывает логи

.EXAMPLE
    .\build-and-monitor.ps1 -Debug -Platform x86
    Запускает сборку Debug x86 и показывает логи
#>

param(
    [switch]$Debug,
    [ValidateSet("x64", "x86", "AnyCPU")]
    [string]$Platform = "x64",
    [string]$Branch = "main",
    [string]$Token
)

# Конфигурация
$RepoOwner = "HypsyNZ"
$RepoName = "BinanceTrader.NET"
$Configuration = if ($Debug) { "Debug" } else { "Release" }

Write-Host "🚀 Запуск сборки Binance Trader на GitHub" -ForegroundColor Cyan
Write-Host "Конфигурация: $Configuration" -ForegroundColor White
Write-Host "Платформа: $Platform" -ForegroundColor White
Write-Host "Ветка: $Branch" -ForegroundColor White

# Получаем токен
function Get-GitHubToken {
    if ($Token) {
        return $Token
    }
    
    $envToken = $env:GITHUB_TOKEN
    if ($envToken) {
        return $envToken
    }
    
    Write-Error "❌ GitHub Token не найден!"
    Write-Host "Укажите параметр -Token или установите переменную окружения GITHUB_TOKEN" -ForegroundColor Yellow
    exit 1
}

# Получаем информацию о репозитории
function Get-CurrentRepository {
    try {
        $remoteUrl = git config --get remote.origin.url
        if ($remoteUrl -match "github\.com[:/]([^/]+)/([^/]+?)(?:\.git)?$") {
            $RepoOwner = $matches[1]
            $RepoName = $matches[2]
            Write-Host "Репозиторий: $RepoOwner/$RepoName" -ForegroundColor Green
            return @{ Owner = $RepoOwner; Name = $RepoName }
        }
    }
    catch {
        Write-Warning "Не удалось определить репозиторий из git config"
    }
    
    return @{ Owner = $RepoOwner; Name = $RepoName }
}

# Запускаем сборку
function Start-Build {
    $token = Get-GitHubToken
    $repo = Get-CurrentRepository
    
    $headers = @{
        "Authorization" = "token $token"
        "Accept" = "application/vnd.github.v3+json"
        "User-Agent" = "BinanceTrader-Build-Monitor"
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
    $url = "https://api.github.com/repos/$($repo.Owner)/$($repo.Name)/actions/workflows/manual-build.yml/dispatches"
    
    Write-Host "`n📡 Запуск сборки..." -ForegroundColor Yellow
    
    try {
        $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $jsonBody -ContentType "application/json"
        Write-Host "✅ Сборка запущена!" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Error "❌ Ошибка при запуске сборки: $($_.Exception.Message)"
        return $false
    }
}

# Получаем статус последнего workflow
function Get-WorkflowStatus {
    param($token, $repo)
    
    $headers = @{
        "Authorization" = "token $token"
        "Accept" = "application/vnd.github.v3+json"
    }
    
    $url = "https://api.github.com/repos/$($repo.Owner)/$($repo.Name)/actions/workflows/manual-build.yml/runs?per_page=1"
    
    try {
        $response = Invoke-RestMethod -Uri $url -Method Get -Headers $headers
        if ($response.workflow_runs.Count -gt 0) {
            return $response.workflow_runs[0]
        }
    }
    catch {
        Write-Warning "Не удалось получить статус workflow"
    }
    
    return $null
}

# Получаем логи job
function Get-JobLogs {
    param($token, $repo, $runId, $jobId)
    
    $headers = @{
        "Authorization" = "token $token"
        "Accept" = "application/vnd.github.v3+json"
    }
    
    $url = "https://api.github.com/repos/$($repo.Owner)/$($repo.Name)/actions/runs/$runId/jobs/$jobId/logs"
    
    try {
        $response = Invoke-RestMethod -Uri $url -Method Get -Headers $headers
        return $response
    }
    catch {
        return $null
    }
}

# Мониторинг сборки
function Monitor-Build {
    param($token, $repo)
    
    Write-Host "`n📊 Мониторинг сборки..." -ForegroundColor Cyan
    Write-Host "Нажмите Ctrl+C для остановки мониторинга" -ForegroundColor Yellow
    
    $lastStatus = ""
    $lastLogs = ""
    
    while ($true) {
        try {
            $workflow = Get-WorkflowStatus -token $token -repo $repo
            
            if ($workflow) {
                $currentStatus = "$($workflow.status) - $($workflow.conclusion)"
                
                if ($currentStatus -ne $lastStatus) {
                    Write-Host "`n🔄 Статус: $currentStatus" -ForegroundColor $(if ($workflow.status -eq "completed") { "Green" } else { "Yellow" })
                    $lastStatus = $currentStatus
                }
                
                # Получаем логи для каждого job
                $jobsUrl = "https://api.github.com/repos/$($repo.Owner)/$($repo.Name)/actions/runs/$($workflow.id)/jobs"
                $headers = @{
                    "Authorization" = "token $token"
                    "Accept" = "application/vnd.github.v3+json"
                }
                
                $jobs = Invoke-RestMethod -Uri $jobsUrl -Method Get -Headers $headers
                
                foreach ($job in $jobs.jobs) {
                    $jobLogs = Get-JobLogs -token $token -repo $repo -runId $workflow.id -jobId $job.id
                    
                    if ($jobLogs -and $jobLogs -ne $lastLogs) {
                        Write-Host "`n📝 Job: $($job.name)" -ForegroundColor Blue
                        Write-Host "Статус: $($job.status) - $($job.conclusion)" -ForegroundColor $(if ($job.conclusion -eq "success") { "Green" } elseif ($job.conclusion -eq "failure") { "Red" } else { "Yellow" })
                        
                        # Показываем последние строки логов
                        $logLines = $jobLogs -split "`n"
                        $recentLines = $logLines | Select-Object -Last 10
                        
                        foreach ($line in $recentLines) {
                            if ($line -match "error|Error|ERROR") {
                                Write-Host "  $line" -ForegroundColor Red
                            } elseif ($line -match "warning|Warning|WARNING") {
                                Write-Host "  $line" -ForegroundColor Yellow
                            } elseif ($line -match "success|Success|SUCCESS|completed|Completed") {
                                Write-Host "  $line" -ForegroundColor Green
                            } else {
                                Write-Host "  $line" -ForegroundColor Gray
                            }
                        }
                        
                        $lastLogs = $jobLogs
                    }
                }
                
                # Проверяем завершение
                if ($workflow.status -eq "completed") {
                    if ($workflow.conclusion -eq "success") {
                        Write-Host "`n🎉 Сборка успешно завершена!" -ForegroundColor Green
                        Write-Host "📋 Ссылка: $($workflow.html_url)" -ForegroundColor Blue
                    } else {
                        Write-Host "`n❌ Сборка завершилась с ошибкой!" -ForegroundColor Red
                        Write-Host "📋 Ссылка: $($workflow.html_url)" -ForegroundColor Blue
                    }
                    break
                }
            }
            
            Start-Sleep -Seconds 5
        }
        catch {
            Write-Warning "Ошибка при мониторинге: $($_.Exception.Message)"
            Start-Sleep -Seconds 10
        }
    }
}

# Основная логика
$token = Get-GitHubToken
$repo = Get-CurrentRepository

if (Start-Build) {
    Write-Host "📋 Ссылка на Actions: https://github.com/$($repo.Owner)/$($repo.Name)/actions" -ForegroundColor Blue
    
    # Небольшая задержка перед началом мониторинга
    Start-Sleep -Seconds 3
    
    Monitor-Build -token $token -repo $repo
}

Write-Host "`n🏁 Скрипт завершен!" -ForegroundColor Cyan 