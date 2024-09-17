using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;


namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                ServiceCollection serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);

                _serviceProvider = serviceCollection.BuildServiceProvider();

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

    }
}
