using System;
using System.Collections.Generic;
using Avalonia_RandomAnimeTorrentApp.ViewModels;
using Avalonia_RandomAnimeTorrentApp.Views;
using Avalonia.Controls;

namespace Avalonia_RandomAnimeTorrentApp.Services;

public interface INavigationService
{
    void NavigateTo(Type viewModelType, object parameter = null);
}

public class NavigationService : INavigationService
{
    private readonly Dictionary<Type, Type> _viewModelMappings;
    private readonly MainWindow _mainWindow; // Ajoutez une référence à la fenêtre principale

    public NavigationService(MainWindow mainWindow) // Passez la fenêtre principale en tant que paramètre au constructeur
    {
        _mainWindow = mainWindow;

        // Initialisez les mappings entre les ViewModels et les vues correspondantes
        _viewModelMappings = new Dictionary<Type, Type>
        {
            { typeof(SearchAndInfoViewModel), typeof(SearchAndInfoView) },
            { typeof(AnimeInfoViewModel), typeof(AnimeInfoView) },
            { typeof(PlayerViewModel), typeof(PlayerView)}
        };
    }

    public void NavigateTo(Type viewModelType, object parameter = null)
    {
        if (!_viewModelMappings.TryGetValue(viewModelType, out Type viewType))
        {
            throw new ArgumentException($"No view mapped for ViewModel type '{viewModelType.FullName}'.");
        }

        // Créez une instance de la vue correspondante
        UserControl view = (UserControl)Activator.CreateInstance(viewType);

        // Assurez-vous que le ViewModel est correctement configuré comme DataContext de la vue
        view.DataContext = Activator.CreateInstance(viewModelType, parameter);

        // Remplacez le contenu de la fenêtre principale par la vue correspondante
        _mainWindow.Content = view;
    }
}
