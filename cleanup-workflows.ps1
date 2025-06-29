# Скрипт для удаления старых workflow файлов из истории Git

Write-Host "Очистка истории от старых workflow файлов..." -ForegroundColor Yellow

# Список файлов для удаления
$filesToRemove = @(
    ".github/workflows/code-quality.yml",
    ".github/workflows/dotnet-desktop.yml", 
    ".github/workflows/manual-build.yml",
    ".github/workflows/version-update.yml"
)

# Создаем команду для filter-branch
$filterCommand = "git filter-branch --force --index-filter `"git rm --cached --ignore-unmatch " + ($filesToRemove -join " ") + "`" --prune-empty --tag-name-filter cat -- --all"

Write-Host "Выполняем команду: $filterCommand" -ForegroundColor Cyan

# Выполняем команду
Invoke-Expression $filterCommand

Write-Host "Очистка завершена!" -ForegroundColor Green
Write-Host "Теперь нужно принудительно отправить изменения: git push --force" -ForegroundColor Yellow 