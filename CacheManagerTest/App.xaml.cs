using DSoft.CacheManager;
using DSoft.CacheManager.LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Mvvm;
using System.Threading.Tasks;
using System.Windows;

namespace CacheManagerTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ServiceHost.Host = Host.CreateDefaultBuilder()
                   .ConfigureServices((context, services) =>
                   {
                       ConfigureServices(context.Configuration, services);
                   })
                   .Build();
        }

        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddScoped<ICacheManager, CacheManager>();
            services.AddTransient<ICacheStorageBackend, LiteDbBackend>(x => new LiteDbBackend(new LiteDbStorageOptions()
            {
                Location = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = "Settings.db",
                Password = "1234567890",
            }));



        }
    }
}
