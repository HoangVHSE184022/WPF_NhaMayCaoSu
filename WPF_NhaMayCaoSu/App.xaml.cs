using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Connect to MQTT broker at startup
            var mqttService = _serviceProvider.GetRequiredService<IMqttService>();
            mqttService.ConnectAsync().Wait();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register services here
            services.AddSingleton<IMqttService, MqttService>();

            // Register the MainWindow
            services.AddSingleton<MainWindow>();
        }
    }

}
