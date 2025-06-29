#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Проверка статуса текущей сборки
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
    Write-Error "GitHub Token не найден!"
    exit 1
}

Write-Host "Проверка статуса сборки в репозитории: $RepoOwner/$RepoName" -ForegroundColor Cyan

$headers = @{
    "Authorization" = "token $Token"
    "Accept" = "application/vnd.github.v3+json"
}

try {
    # Получаем последний workflow run
    $url = "https://api.github.com/repos/$RepoOwner/$RepoName/actions/workflows/manual-build.yml/runs?per_page=1"
    $response = Invoke-RestMethod -Uri $url -Method Get -Headers $headers
    
    if ($response.workflow_runs.Count -gt 0) {
        $workflow = $response.workflow_runs[0]
        $currentStatus = "$($workflow.status) - $($workflow.conclusion)"
        
        Write-Host "Статус: $currentStatus" -ForegroundColor $(if ($workflow.status -eq "completed") { "Green" } else { "Yellow" })
        Write-Host "Создан: $($workflow.created_at)" -ForegroundColor Gray
        Write-Host "Ссылка: $($workflow.html_url)" -ForegroundColor Blue
        
        # Получаем информацию о jobs
        $jobsUrl = "https://api.github.com/repos/$RepoOwner/$RepoName/actions/runs/$($workflow.id)/jobs"
        $jobs = Invoke-RestMethod -Uri $jobsUrl -Method Get -Headers $headers
        
        Write-Host "`nJobs:" -ForegroundColor Cyan
        foreach ($job in $jobs.jobs) {
            $jobStatus = "$($job.status) - $($job.conclusion)"
            Write-Host "  Job: $($job.name) - $jobStatus" -ForegroundColor $(if ($job.conclusion -eq "success") { "Green" } elseif ($job.conclusion -eq "failure") { "Red" } else { "Yellow" })
        }
        
        # Проверяем завершение
        if ($workflow.status -eq "completed") {
            if ($workflow.conclusion -eq "success") {
                Write-Host "`nСборка успешно завершена!" -ForegroundColor Green
                
                # Получаем информацию об артефактах
                $artifactsUrl = "https://api.github.com/repos/$RepoOwner/$RepoName/actions/runs/$($workflow.id)/artifacts"
                try {
                    $artifacts = Invoke-RestMethod -Uri $artifactsUrl -Method Get -Headers $headers
                    if ($artifacts.artifacts.Count -gt 0) {
                        Write-Host "Созданные артефакты:" -ForegroundColor Cyan
                        foreach ($artifact in $artifacts.artifacts) {
                            Write-Host "  $($artifact.name) - $([math]::Round($artifact.size_in_bytes / 1MB, 2)) MB" -ForegroundColor White
                        }
                    }
                }
                catch {
                    Write-Host "Не удалось получить информацию об артефактах" -ForegroundColor Yellow
                }
                
                return $true
            } else {
                Write-Host "`nСборка завершилась с ошибкой!" -ForegroundColor Red
                return $false
            }
        } else {
            Write-Host "`nСборка еще выполняется..." -ForegroundColor Yellow
            return $null
        }
    } else {
        Write-Host "Сборки не найдены" -ForegroundColor Yellow
        return $null
    }
}
catch {
    Write-Error "Ошибка при проверке статуса: $($_.Exception.Message)"
    return $null
} 