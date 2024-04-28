using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Newtonsoft.Json.Linq;
using System.Linq;
using Avalonia.Threading;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using System.Net;
using Avalonia_RandomAnimeTorrentApp.Models;
using Avalonia_RandomAnimeTorrentApp.ViewModels;
using Avalonia_RandomAnimeTorrentApp.DataAccess;
using CommunityToolkit.Mvvm.Messaging;
using static GraphQL.Validation.Rules.OverlappingFieldsCanBeMerged;

namespace Avalonia_RandomAnimeTorrentApp.Views
{
    public partial class SearchAndInfoView : UserControl
    {

        #region private members

        private ListBox mSearchResultsListBox;
        private TextBox mSearchTextBox;
        private Grid mGridSearchResultsListBox;
        private AnimeInfoView animeInfo;

        #endregion
        //public ObservableCollection<string> MyList { get; } = new ObservableCollection<string>();

        public SearchAndInfoView()
        {
            InitializeComponent();

            //DataContext = new SearchAndInfoViewModel();

            //idk some shit needed
            //mSearchResultsListBox = this.FindControl<ListBox>("SearchResultsListBox") ?? throw new Exception("SearchResultsListBox not found");
            mSearchTextBox = this.FindControl<TextBox>("SearchTextBox") ?? throw new Exception("SearchTextBox not found");
            //mGridSearchResultsListBox = this.FindControl<Grid>("GridSearchResultsListBox") ?? throw new Exception("GridSearchResultsListBox not found");
            mSearchTextBox.Focus();
            //mSearchTextBox.AddHandler(TextInputEvent, TextBoxSearchQuerieUpdate, RoutingStrategies.Tunnel);
        }
        

        private void TextBoxLostFocusEvent(object sender, RoutedEventArgs e)
        {
            //var textBox = sender as TextBox;

            GridSearchResultsListBox.IsVisible = false;
            /*
            if (textBox != null)
            {
                (DataContext as SearchAndInfoViewModel).TextBoxLostFocusAsync(textBox, e);
            }*/
        }

        private void TextBoxGotFocusEvent(object sender, GotFocusEventArgs e)
        {
            //var textBox = sender as TextBox;
            
            GridSearchResultsListBox.IsVisible = true;
            
            /*
            if (textBox != null)
            {
                (DataContext as SearchAndInfoViewModel).TextBoxGotFocusAsync(textBox, e);
            }*/
        }

    }
}
