using CacheManagerTest.Models;
using DSoft.CacheManager;
using DSoft.CacheManager.LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Mvvm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CacheManagerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cacheManager = ServiceHost.GetRequiredService<ICacheManager>();

                var dataKey = typeof(SomeData).Name;

                if (!cacheManager.IsLoaded)
                {
                    await cacheManager.LoadAsync();
                }

                var items = new List<SomeData>()
                                {
                                    new SomeData() { Id = 1, Name = "One", IsEnabled = true },
                                    new SomeData() { Id = 2, Name = "Two", IsEnabled = false },
                                    new SomeData() { Id = 3, Name = "Three", IsEnabled = true },
                                    new SomeData() { Id = 4, Name = "Four", IsEnabled = false },
                                };

                var exists = cacheManager.IsKeyRegistered(dataKey);

                if (exists)
                {
                    var ogItems = cacheManager.GetItems<SomeData>(dataKey);

                    var count = ogItems.Count;
                }

                cacheManager.SetItems(dataKey, items);


                var itemsOut = cacheManager.GetItems<SomeData>(dataKey);

                Debug.WriteLine("");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
            }


        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await ServiceHost.Host.StartAsync();
        }
    }
    
}
