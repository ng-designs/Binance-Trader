#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Мониторинг текущей сборки до завершения
#>

param(
    [string]$Token
)

# Конфигурация
$RepoOwner = "ng-designs"
$RepoName = "Binance-Trader"

# Получаем токен
if (-not $Token) {
    $Token = $env:GITHUB_TOKEN
}

if (-not $Token) {
    Write-Error "❌ GitHub Token не найден!"
    exit 1
}

Write-Host "📊 Мониторинг сборки в репозитории: $RepoOwner/$RepoName" -ForegroundColor Cyan
Write-Host "Нажмите Ctrl+C для остановки мониторинга" -ForegroundColor Yellow

$headers = @{
    "Authorization" = "token $Token"
    "Accept" = "application/vnd.github.v3+json"
}

$lastStatus = ""
$lastLogs = ""

while ($true) {
    try {
        # Получаем последний workflow run
        $url = "https://api.github.com/repos/$RepoOwner/$RepoName/actions/workflows/manual-build.yml/runs?per_page=1"
        $response = Invoke-RestMethod -Uri $url -Method Get -Headers $headers
        
        if ($response.workflow_runs.Count -gt 0) {
            $workflow = $response.workflow_runs[0]
            $currentStatus = "$($workflow.status) - $($workflow.conclusion)"
            
            if ($currentStatus -ne $lastStatus) {
                Write-Host "`n🔄 Статус: $currentStatus" -ForegroundColor $(if ($workflow.status -eq "completed") { "Green" } else { "Yellow" })
                Write-Host "🕐 Создан: $($workflow.created_at)" -ForegroundColor Gray
                Write-Host "🔗 Ссылка: $($workflow.html_url)" -ForegroundColor Blue
                $lastStatus = $currentStatus
            }
            
            # Получаем информацию о jobs
            $jobsUrl = "https://api.github.com/repos/$RepoOwner/$RepoName/actions/runs/$($workflow.id)/jobs"
            $jobs = Invoke-RestMethod -Uri $jobsUrl -Method Get -Headers $headers
            
            foreach ($job in $jobs.jobs) {
                $jobStatus = "$($job.status) - $($job.conclusion)"
                Write-Host "  📝 Job: $($job.name) - $jobStatus" -ForegroundColor $(if ($job.conclusion -eq "success") { "Green" } elseif ($job.conclusion -eq "failure") { "Red" } else { "Yellow" })
                
                # Получаем логи job если он завершился
                if ($job.status -eq "completed") {
                    $logsUrl = "https://api.github.com/repos/$RepoOwner/$RepoName/actions/runs/$($workflow.id)/jobs/$($job.id)/logs"
                    $jobLogs = Invoke-RestMethod -Uri $logsUrl -Method Get -Headers $headers
                    
                    if ($jobLogs -and $jobLogs -ne $lastLogs) {
                        $logLines = $jobLogs -split "`n"
                        $recentLines = $logLines | Select-Object -Last 5
                        
                        foreach ($line in $recentLines) {
                            if ($line -match "error|Error|ERROR") {
                                Write-Host "    ❌ $line" -ForegroundColor Red
                            } elseif ($line -match "warning|Warning|WARNING") {
                                Write-Host "    ⚠️ $line" -ForegroundColor Yellow
                            } elseif ($line -match "success|Success|SUCCESS|completed|Completed") {
                                Write-Host "    ✅ $line" -ForegroundColor Green
                            } elseif ($line -match "BUILD SUMMARY|Build Output|Main Executable") {
                                Write-Host "    📋 $line" -ForegroundColor Cyan
                            }
                        }
                        $lastLogs = $jobLogs
                    }
                }
            }
            
            # Проверяем завершение
            if ($workflow.status -eq "completed") {
                if ($workflow.conclusion -eq "success") {
                    Write-Host "`n🎉 Сборка успешно завершена!" -ForegroundColor Green
                    Write-Host "📋 Ссылка на результаты: $($workflow.html_url)" -ForegroundColor Blue
                    
                    # Получаем информацию об артефактах
                    $artifactsUrl = "https://api.github.com/repos/$RepoOwner/$RepoName/actions/runs/$($workflow.id)/artifacts"
                    try {
                        $artifacts = Invoke-RestMethod -Uri $artifactsUrl -Method Get -Headers $headers
                        if ($artifacts.artifacts.Count -gt 0) {
                            Write-Host "📦 Созданные артефакты:" -ForegroundColor Cyan
                            foreach ($artifact in $artifacts.artifacts) {
                                Write-Host "  📁 $($artifact.name) - $([math]::Round($artifact.size_in_bytes / 1MB, 2)) MB" -ForegroundColor White
                            }
                        }
                    }
                    catch {
                        Write-Host "⚠️ Не удалось получить информацию об артефактах" -ForegroundColor Yellow
                    }
                    
                    break
                } else {
                    Write-Host "`n❌ Сборка завершилась с ошибкой!" -ForegroundColor Red
                    Write-Host "📋 Ссылка на логи: $($workflow.html_url)" -ForegroundColor Blue
                    break
                }
            }
        } else {
            Write-Host "⏳ Ожидание запуска сборки..." -ForegroundColor Yellow
        }
        
        Start-Sleep -Seconds 10
    }
    catch {
        Write-Warning "Ошибка при мониторинге: $($_.Exception.Message)"
        Start-Sleep -Seconds 15
    }
}

Write-Host "`n🏁 Мониторинг завершен!" -ForegroundColor Cyan 