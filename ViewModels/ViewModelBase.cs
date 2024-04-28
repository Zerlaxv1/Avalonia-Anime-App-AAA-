using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private string title = "Avalonia Random Anime Torrent App";

        [ObservableProperty]
        private string version = "v0.0.0.0.01";

        [ObservableProperty]
        private IBrush backgroundBrush = new SolidColorBrush(Colors.Gray);
    }
}