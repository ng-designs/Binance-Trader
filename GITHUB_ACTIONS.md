# GitHub Actions для Binance Trader

Этот документ описывает созданные GitHub Actions workflows для автоматизации сборки, тестирования и развертывания проекта Binance Trader.

## Обзор Workflows

### 1. Build and Release (`build.yml`)

Основной workflow для сборки и релиза приложения.

**Триггеры:**
- Push в ветки: `main`, `master`, `develop`
- Push тегов: `v*`
- Pull Request в ветки: `main`, `master`

**Jobs:**
- **build**: Сборка для разных конфигураций (Release/Debug) и платформ (x64/x86/AnyCPU)
- **test**: Запуск тестов (если есть)
- **package**: Создание релизных пакетов (только для тегов)
- **release**: Создание GitHub Release (только для тегов)
- **security**: Сканирование безопасности с CodeQL и Trivy

**Особенности:**
- Использует .NET Framework 4.8
- MSBuild 17.0
- Создает артефакты для каждой конфигурации
- Автоматическое создание релизов при тегах

### 2. Code Quality (`code-quality.yml`)

Workflow для проверки качества кода.

**Триггеры:**
- Push в ветки: `main`, `master`, `develop`
- Pull Request в ветки: `main`, `master`

**Jobs:**
- **code-analysis**: Статический анализ кода с MSBuild и StyleCop
- **sonarqube**: Анализ с SonarCloud
- **dependency-check**: Проверка уязвимостей зависимостей с OWASP

### 3. Version Update (`version-update.yml`)

Workflow для автоматического обновления версий.

**Триггеры:**
- Ручной запуск (workflow_dispatch)

**Входные параметры:**
- `version_type`: Тип обновления версии (major/minor/patch)
- `create_release`: Создать ли GitHub Release

**Jobs:**
- **update-version**: Обновление версии в файлах проекта
- **notify**: Уведомления команды о результатах

## Конфигурационные файлы

### .editorconfig
Настройки стиля кода для единообразия форматирования.

### Directory.Build.props
Общие настройки сборки для всех проектов в решении.

## Использование

### Автоматическая сборка
1. Сделайте push в основную ветку
2. Workflow автоматически запустится
3. Проверьте результаты в Actions tab

### Создание релиза
1. Создайте тег: `git tag v2.8.1.5`
2. Push тег: `git push origin v2.8.1.5`
3. Workflow автоматически создаст релиз

### Обновление версии
1. Перейдите в Actions tab
2. Выберите "Version Update" workflow
3. Нажмите "Run workflow"
4. Выберите тип обновления версии
5. Нажмите "Run workflow"

## Настройка

### Необходимые Secrets
Для полной функциональности настройте следующие secrets в репозитории:

- `SONAR_TOKEN`: Токен для SonarCloud (опционально)
- `GITHUB_TOKEN`: Автоматически предоставляется GitHub

### Настройка SonarCloud
1. Создайте аккаунт на SonarCloud
2. Создайте проект для репозитория
3. Получите токен и добавьте в secrets
4. Обновите `sonar.organization` в workflow

## Структура артефактов

### Build Artifacts
- `binance-trader-Release-x64.zip`
- `binance-trader-Release-x86.zip`
- `binance-trader-Release-AnyCPU.zip`
- `binance-trader-Debug-x64.zip`

### Release Package
- `release-package/`: Основной релиз (x64)
- `release-package-portable/`: Портативная версия
- `release-package-installer/`: Все версии для установки

## Мониторинг

### Уведомления
- Успешные сборки: Создается issue с задачами
- Неудачные сборки: Создается issue с ошибкой
- Обновления версий: Уведомления в issues

### Логи
Все логи доступны в Actions tab GitHub репозитория.

## Требования

- .NET Framework 4.8 SDK
- MSBuild 17.0
- Windows runner для сборки
- Ubuntu runner для некоторых задач

## Troubleshooting

### Частые проблемы

1. **Ошибка сборки .NET Framework**
   - Убедитесь, что установлен .NET Framework 4.8 SDK
   - Проверьте версию MSBuild

2. **Проблемы с NuGet**
   - Проверьте файл `nuget.config`
   - Убедитесь в доступности пакетов

3. **Ошибки SonarCloud**
   - Проверьте токен в secrets
   - Убедитесь в правильности настроек проекта

### Логи и отладка
- Все шаги имеют подробные логи
- Используйте `continue-on-error: true` для некритичных шагов
- Проверяйте артефакты для дополнительной информации 