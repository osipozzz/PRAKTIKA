using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WpfApp.Data;
using WpfApp.Services;
using WpfApp.ViewModels;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            // Настройка логирования Serilog
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "app-.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // Настройка DI
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            Log.Logger.Information("Приложение запущено");
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Логирование
            services.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            // База данных
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products.db");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Сервисы
            services.AddScoped<IDataService, DataService>();

            // ViewModels
            services.AddTransient<MainViewModel>();

            // Главное окно
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Инициализация БД
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
                Log.Logger.Information("База данных инициализирована");

                // Показ главного окна
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

                Log.Logger.Information("Главное окно показано");
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Критическая ошибка при запуске приложения");
                MessageBox.Show(
                    $"Произошла критическая ошибка при запуске приложения:\n\n{ex.Message}",
                    "Ошибка запуска",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Logger.Information("Приложение завершено");
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Logger.Fatal(e.Exception, "Необработанное исключение WPF");
            MessageBox.Show(
                $"Произошла непредвиденная ошибка:\n\n{e.Exception.Message}\n\nПодробности в логе.",
                "Ошибка приложения",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            e.Handled = true;
            Shutdown(1);
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            Log.Logger.Information("Приложение закрыто с кодом: {Code}", e.ApplicationExitCode);
        }
    }
}
