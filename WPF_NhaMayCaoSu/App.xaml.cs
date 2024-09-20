using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using Serilog;
using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;


namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public App()
        {
             LoggingHelper.ConfigureLogger();
            // Log the application startup
            Log.Information("Application is starting up");
        }


        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                // Log the unhandled exception
                Exception ex = (Exception)args.ExceptionObject;
                Log.Error(ex, "An unhandled exception occurred");
            };

            this.DispatcherUnhandledException += (sender, args) =>
            {
                // Log UI thread exceptions
                Log.Error(args.Exception, "An unhandled UI exception occurred");
                args.Handled = true; 
            };
            try
            {
                ServiceCollection serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);

                _serviceProvider = serviceCollection.BuildServiceProvider();

                InitializeDatabase();

                BrokerWindow brokerWindow = _serviceProvider.GetRequiredService<BrokerWindow>();
                //mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                Shutdown();
            }
        }



        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CaoSuWpfDbContext>(options =>
                options.UseSqlServer(
                    new CaoSuWpfDbContext().GetConnectionString()));

            // Register services here
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddSingleton<IMqttServerService, MqttServerService>();
            services.AddSingleton<IMqttClientService, MqttClientService>();
            // Register repositories here
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            // Register the MainWindow
            services.AddScoped<MainWindow>();
            services.AddScoped<LoginWindow>();
            services.AddScoped<BrokerWindow>();
        }
        private void InitializeDatabase()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CaoSuWpfDbContext>();
                try
                {
                    // Apply any pending migrations or create the database if it doesn't exist
                    dbContext.Database.Migrate();
                    Log.Information("Database initialized successfully.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while initializing the database.");
                    MessageBox.Show($"Database initialization failed: {ex.Message}");
                    Shutdown();
                }
            }
        }
    }
}
