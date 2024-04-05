using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfApp.ViewModels;
using WpfApp.Views;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public DisplayRootRegistry displayRootRegistry { get; set; } = new DisplayRootRegistry();
        ViewModel _mainWindowViewModel;

        public App() 
        {
            displayRootRegistry.RegisterWindowType<ViewModel, MainWindow>();
            displayRootRegistry.RegisterWindowType<ViewModelReport, DialogWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _mainWindowViewModel = new ViewModel();

            await displayRootRegistry.ShowModalPresentation(_mainWindowViewModel);

            Shutdown();
        }
    }
}
