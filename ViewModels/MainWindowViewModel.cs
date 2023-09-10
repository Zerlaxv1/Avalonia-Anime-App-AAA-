using Avalonia_RandomAnimeTorrentApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.IO;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private object content;

        readonly SearchAndInfoViewModel saivm = new();

        readonly PlayerViewModel pvm = new();

        public MainWindowViewModel()
        {
            Content = saivm;

            WeakReferenceMessenger.Default.Register<Stream>(this, (r, m) =>
            {
                Content = pvm;
            });
        }
        
    }
}