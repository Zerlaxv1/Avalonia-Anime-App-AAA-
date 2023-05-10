using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels 
{
    internal partial class SearchAndInfoViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<String> searchResults = new List<String>();

        [ObservableProperty]
        private System.Timers.Timer searchTimer;

        [ObservableProperty]
        private string greeting = "Present ?";

        [ObservableProperty]
        private bool isGridListBoxVisible = false;

        [ObservableProperty]
        public string searchText;

        [ObservableProperty]
        private string myItems;

        [ObservableProperty]
        private string imageUrl = "https://i.imgur.com/1Z1Z1Z1.png";

        [RelayCommand]
        private void TextBoxGotFocus() => isGridListBoxVisible = true;

        [RelayCommand]
        private void TextBoxLostFocus() => isGridListBoxVisible = false;

        [RelayCommand]
        private void TextBoxTextInput()
        {

        }

        [RelayCommand]
        private void onSearchResulteSelectionChangedListBox() { }
    }
}
