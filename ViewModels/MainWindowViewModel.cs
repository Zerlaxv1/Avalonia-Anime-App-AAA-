using Avalonia_RandomAnimeTorrentApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System;
using System.IO;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private Dictionary<Type, object> viewModelCache = new Dictionary<Type, object>();

        [ObservableProperty]
        private object? content;

        public MainWindowViewModel()
        {
            ChangeContent<SearchAndInfoViewModel>();

            WeakReferenceMessenger.Default.Register<Stream>(this, (r, m) =>
            {
                ChangeContent<PlayerViewModel>(m);
            });
        }
        private void ChangeContent<T>(Stream stream = null) where T : class
        {
            Type type = typeof(T);

            if (viewModelCache.TryGetValue(type, out object? cachedViewModel))
            {
                // Use the cached ViewModel
                Content = cachedViewModel;
            }
            else
            {
                // Create a new ViewModel
                if (type == typeof(SearchAndInfoViewModel))
                {
                    viewModelCache[type] = new SearchAndInfoViewModel();
                }
                else if (type == typeof(PlayerViewModel))
                {
                    viewModelCache[type] = new PlayerViewModel(stream);
                }

                Content = viewModelCache[type];
            }
        }

    }
}