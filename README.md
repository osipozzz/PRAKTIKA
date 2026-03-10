# 📦 Управление товарами (WPF + SQLite)

Приложение для управления товарами с использованием **WPF**, **MVVM архитектуры**, **Entity Framework Core** и **SQLite**.

---

## 🚀 Возможности

- ✅ **CRUD операции** — добавление, редактирование, удаление товаров
- 🔍 **Поиск** — поиск товаров по названию и описанию
- 💾 **База данных** — SQLite для локального хранения данных
- 🏗️ **MVVM архитектура** — чистое разделение логики и UI
- 🔧 **Dependency Injection** — внедрение зависимостей через Microsoft.Extensions.DependencyInjection
- 📝 **Валидация** — валидация данных на уровне ViewModel
- 📊 **Логирование** — Serilog для записи логов в файлы

---

## 📋 Требования

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11

---

## 🛠️ Установка и запуск

### 1. Клонирование репозитория

```bash
git clone https://github.com/yourusername/WpfApp.git
cd WpfApp
```

### 2. Восстановление пакетов

```bash
dotnet restore
```

### 3. Запуск приложения

```bash
dotnet run
```

### 4. Сборка релизной версии

```bash
dotnet publish -c Release -o ./publish
```

---

## 📁 Структура проекта

```
WpfApp/
├── Core/                      # Базовые классы MVVM
│   ├── ViewModelBase.cs       # Базовый класс для ViewModel
│   └── RelayCommand.cs        # Команды для привязок
├── ViewModels/                # ViewModel слой
│   └── MainViewModel.cs       # Главная ViewModel
├── Models/                    # Модели данных
│   └── Product.cs             # Модель товара
├── Services/                  # Сервисный слой
│   ├── IDataService.cs        # Интерфейс сервиса данных
│   └── DataService.cs         # Реализация сервиса
├── Data/                      # Работа с данными
│   └── AppDbContext.cs        # EF Core контекст
├── Converters/                # Конвертеры для XAML
│   └── Converters.cs
├── Assets/                    # Ресурсы приложения
│   └── app.ico                # Иконка приложения
├── App.xaml(.cs)              # Точка входа, DI, логирование
├── MainWindow.xaml(.cs)       # Главное окно
└── WpfApp.csproj              # Проект
```

---

## 🎯 Архитектура

### Dependency Injection

Все зависимости регистрируются в `App.xaml.cs`:

```csharp
services.AddDbContext<AppDbContext>(...);
services.AddScoped<IDataService, DataService>();
services.AddTransient<MainViewModel>();
services.AddSingleton<MainWindow>();
```

---

## 🔧 Конфигурация

### База данных

База данных SQLite создаётся автоматически в папке приложения:

```
bin/Debug/net8.0-windows/products.db
```

### Логирование

Логи записываются в папку `Logs/` с ежедневной ротацией:

```
Logs/app-20260310.log
Logs/app-20260311.log
...
```

Настройки логирования в `App.xaml.cs`:

- Минимальный уровень: **Information**
- Хранение: **7 дней**
- Формат: `{Timestamp} [{Level}] {Message}{NewLine}{Exception}`

---

## 📝 Модели данных

### Product

| Поле        | Тип     | Описание                          |
| ----------- | ------- | --------------------------------- |
| Id          | int     | Уникальный идентификатор          |
| Name        | string  | Название товара (до 100 символов) |
| Price       | decimal | Цена (18,2)                       |
| Quantity    | int     | Количество на складе              |
| Description | string? | Описание (до 500 символов)        |

---

## 🎮 Использование

### Добавление товара

1. Заполните поля формы:
   - **Название** — обязательное поле
   - **Цена** — неотрицательное число
   - **Количество** — неотрицательное целое
   - **Описание** — опционально
2. Нажмите **«Добавить»**

### Редактирование товара

1. Выберите товар в списке
2. Измените данные в форме
3. Нажмите **«Изменить»**

### Удаление товара

1. Выберите товар в списке
2. Нажмите **«Удалить»**
3. Подтвердите удаление

### Поиск товаров

1. Введите текст в поле поиска
2. Фильтрация происходит автоматически
3. Нажмите **«Очистить»** для сброса

---

## 🧪 Тестирование

Для запуска тестов (если добавлены):

```bash
dotnet test
```

---

## 📦 Зависимости

| Пакет                                    | Версия | Назначение          |
| ---------------------------------------- | ------ | ------------------- |
| Microsoft.EntityFrameworkCore.Sqlite     | 8.0.0  | ORM для SQLite      |
| Microsoft.EntityFrameworkCore.Design     | 8.0.0  | Инструменты EF      |
| Microsoft.Extensions.DependencyInjection | 10.0.3 | DI контейнер        |
| Serilog.Extensions.Logging               | 10.0.0 | Логирование         |
| Serilog.Sinks.File                       | 7.0.0  | Запись логов в файл |
| System.Drawing.Common                    | 8.0.0  | Работа с графикой   |

---

## 🔐 Безопасность

- Локальное хранение данных (SQLite)
- Валидация входных данных
- Обработка исключений с логированием

---

## 👨‍💻 Разработка

### Добавление новой функциональности

1. Создайте ветку:

   ```bash
   git checkout -b feature/new-feature
   ```

2. Внесите изменения

3. Закоммитьте:

   ```bash
   git commit -m "feat: описание изменений"
   ```

4. Отправьте:
   ```bash
   git push origin feature/new-feature
   ```

### Соглашения по коду

- **C#** — PascalCase для классов, методов, свойств
- **Private поля** — \_camelCase с подчёркиванием
- **ViewModel** — суффикс `ViewModel`
- **Команды** — суффикс `Command`
- **Методы команд** — префикс `Can` для CanExecute

---

## 🐛 Решение проблем

### Приложение не запускается

```bash
# Очистите и пересоберите
dotnet clean
dotnet build
dotnet run
```

### Ошибки базы данных

Удалите файл `products.db` — база создастся заново при запуске.

### Проблемы с пакетами

```bash
dotnet restore --force
```
