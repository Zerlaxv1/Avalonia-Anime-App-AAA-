using System;
using Microsoft.Extensions.DependencyInjection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia_RandomAnimeTorrentApp.ViewModels;
using Avalonia_RandomAnimeTorrentApp.Views;
using Avalonia.SimpleRouter;

namespace Avalonia_RandomAnimeTorrentApp
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            IServiceProvider services = ConfigureServices();
            var mainViewModel = services.GetRequiredService<MainWindowViewModel>();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<HistoryRouter<ViewModelBase>>(
                s => new HistoryRouter<ViewModelBase>(
                    t => (ViewModelBase)s.GetRequiredService(t)));

            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<SearchAndInfoViewModel>();
            services.AddTransient<PlayerViewModel>();
            services.AddTransient<AnimeInfoViewModel>();
            return services.BuildServiceProvider();
        }
    }
}