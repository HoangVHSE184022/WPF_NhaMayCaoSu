using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<ISaleService, SaleService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IRoleService, RoleService>();
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<IMqttServerService, MqttServerService>();
            services.AddSingleton<IMqttClientService, MqttClientService>();
            // Register repositories here
            services.AddSingleton<ISaleRepository, SaleRepository>();
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IRoleRepository, RoleRepository>();
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            // Register the MainWindow
            services.AddSingleton<MainWindow>();
            services.AddSingleton<LoginWindow>();
            services.AddSingleton<BrokerWindow>();
        }

    }
}
