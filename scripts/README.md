# Скрипты для сборки Binance Trader

Этот каталог содержит скрипты для автоматизации сборки проекта через GitHub Actions.

## Быстрый старт

### 1. Установка GitHub Token

Создайте Personal Access Token на GitHub:
1. Перейдите в Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Нажмите "Generate new token"
3. Выберите scopes: `repo`, `workflow`
4. Скопируйте токен

### 2. Настройка переменной окружения

```powershell
# Windows PowerShell
$env:GITHUB_TOKEN = "your_token_here"

# Или добавьте в системные переменные окружения
```

### 3. Запуск сборки

#### Простой способ (bat файл)
```cmd
build.bat
```

#### PowerShell скрипт с мониторингом
```powershell
.\scripts\build-and-monitor.ps1
```

#### С параметрами
```powershell
.\scripts\build-and-monitor.ps1 -Debug -Platform x86
```

## Доступные скрипты

### build-and-monitor.ps1
Основной скрипт для запуска сборки с мониторингом в реальном времени.

**Параметры:**
- `-Debug` - сборка в Debug конфигурации
- `-Platform` - платформа (x64, x86, AnyCPU)
- `-Branch` - ветка для сборки
- `-Token` - GitHub токен

**Примеры:**
```powershell
# Release x64 (по умолчанию)
.\scripts\build-and-monitor.ps1

# Debug x86
.\scripts\build-and-monitor.ps1 -Debug -Platform x86

# С указанием токена
.\scripts\build-and-monitor.ps1 -Token "ghp_xxxxxxxx"
```

### trigger-github-actions.ps1
Расширенный скрипт для запуска различных GitHub Actions workflows.

**Действия:**
- `build` - запуск сборки
- `quality` - проверка качества кода
- `version` - обновление версии
- `release` - создание релиза

**Примеры:**
```powershell
# Запуск сборки
.\scripts\trigger-github-actions.ps1 -Action build

# Обновление версии
.\scripts\trigger-github-actions.ps1 -Action version -VersionType patch -CreateRelease

# Проверка качества кода
.\scripts\trigger-github-actions.ps1 -Action quality -Branch develop
```

### quick-build.ps1
Простой скрипт для быстрого запуска сборки.

**Примеры:**
```powershell
# Release x64
.\scripts\quick-build.ps1

# Debug x64
.\scripts\quick-build.ps1 -Debug

# Release x86
.\scripts\quick-build.ps1 -Platform x86
```

## Мониторинг сборки

Скрипт `build-and-monitor.ps1` показывает:

- ✅ Статус сборки в реальном времени
- 📝 Логи каждого job
- 🎨 Цветная подсветка ошибок и предупреждений
- 🔗 Ссылки на GitHub Actions

**Цветовая схема:**
- 🔴 Красный - ошибки
- 🟡 Желтый - предупреждения
- 🟢 Зеленый - успех
- 🔵 Синий - информация
- ⚪ Серый - обычные сообщения

## Troubleshooting

### Ошибка "GitHub Token не найден"
```powershell
# Установите переменную окружения
$env:GITHUB_TOKEN = "your_token_here"

# Или передайте токен в параметре
.\scripts\build-and-monitor.ps1 -Token "your_token_here"
```

### Ошибка "Execution Policy"
```powershell
# Разрешите выполнение скриптов
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Ошибка "Repository not found"
Проверьте:
1. Правильность токена
2. Права доступа к репозиторию
3. Название репозитория в скрипте

### Сборка не запускается
Проверьте:
1. Наличие workflow файла `.github/workflows/manual-build.yml`
2. Правильность названия workflow
3. Права токена на запуск workflows

## Интеграция с IDE

### Visual Studio Code
Добавьте в `tasks.json`:
```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Build on GitHub",
            "type": "shell",
            "command": "powershell",
            "args": [
                "-ExecutionPolicy", "Bypass",
                "-File", "${workspaceFolder}/scripts/build-and-monitor.ps1"
            ],
            "group": "build",
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "new"
            }
        }
    ]
}
```

### Visual Studio
Добавьте внешний инструмент:
1. Tools → External Tools
2. Add: `powershell.exe`
3. Arguments: `-ExecutionPolicy Bypass -File "$(SolutionDir)scripts\build-and-monitor.ps1"`

## Автоматизация

### Git Hooks
Добавьте в `.git/hooks/pre-push`:
```bash
#!/bin/sh
echo "Запуск сборки на GitHub..."
powershell -ExecutionPolicy Bypass -File "scripts/build-and-monitor.ps1"
```

### CI/CD Pipeline
Используйте скрипты в ваших CI/CD pipeline для автоматического тестирования сборки.

## Поддержка

При возникновении проблем:
1. Проверьте логи в GitHub Actions
2. Убедитесь в правильности токена
3. Проверьте права доступа к репозиторию
4. Создайте issue в репозитории 