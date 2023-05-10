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

namespace Avalonia_RandomAnimeTorrentApp.Views
{
    public partial class SearchAndInfoView : UserControl
    {

        #region private members

        private ListBox mSearchResultsListBox;
        private TextBox mSearchTextBox;
        private Grid mGridSearchResultsListBox;
        private AnimeInfo animeInfo;

        #endregion

        private Stopwatch stopwatch = new Stopwatch();
        System.Timers.Timer timer;
        private JArray arrayId;
        public ObservableCollection<string> MyList { get; } = new ObservableCollection<string>();

        public SearchAndInfoView()
        {
            InitializeComponent();

            DataContext = new SearchAndInfoViewModel();

            ///idk some shit needed
            mSearchResultsListBox = this.FindControl<ListBox>("SearchResultsListBox") ?? throw new Exception("SearchResultsListBox not found");
            mSearchTextBox = this.FindControl<TextBox>("SearchTextBox") ?? throw new Exception("SearchTextBox not found");
            mGridSearchResultsListBox = this.FindControl<Grid>("GridSearchResultsListBox") ?? throw new Exception("GridSearchResultsListBox not found");
            mSearchTextBox.Focus();
            mSearchTextBox.AddHandler(TextInputEvent, TextBoxSearchQuerieUpdate, RoutingStrategies.Tunnel);

            ///when stop tying for 500ms, search
            timer = new System.Timers.Timer(500);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            timer.Stop();
        }

        private void TextBoxTextInput(object sender, RoutedEventArgs e )
        {
            
        }
        
        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            ///list box invisible
            mGridSearchResultsListBox.IsVisible = false;
        }

        private void TextBoxGotFocus(object sender, GotFocusEventArgs e)
        {
            ///list box visible
            mGridSearchResultsListBox.IsVisible = true;
        }
        
        private void TextBoxSearchQuerieUpdate(object sender, TextInputEventArgs e)
        {
            ///reset timer
            timer.Stop();
            timer.Start();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            ///just moved the function idk why but i did it
            TextBoxSearch(source, e);
        }

        private async void TextBoxSearch(object sender, ElapsedEventArgs e)
        {
            ///stop timer
            timer.Stop();

            ///get the text
            String recherche = mSearchTextBox.Text;

            ///initialise the list
            //List<String> ResultList = new List<String>();

            /// the query to use with graphql
            var query = @"
            query ($search: String) {
  Page(page: 1, perPage: 5) {
    pageInfo {
      total
      currentPage
      lastPage
      hasNextPage
      perPage
    }
    media(search: $search, type: ANIME) {
      id
      title {
        romaji(stylised: true)
        english(stylised: true)
      }
      coverImage {
        extraLarge
        large
        medium
        color
      }
    }
  }
}

            ";

            ///the variables to use with graphql so the research
            var variables = @"
            {""search"": """ + recherche + @"""}";

            ///the url to use with graphql
            Uri url = new Uri("https://graphql.anilist.co");

            ///call the api
            JObject ApiResponse = await WebDb.CallApiGraphQl(query, variables, url) ?? throw new Exception("Anlist api return null");

            ///get the results and filter it a little
            var results = ApiResponse["Page"]["media"].ToList();

            ///initialise some lists
            List<Bitmap> imglist = new List<Bitmap>();
            List<MyItem> items = new List<MyItem>();

            ///set the index to 0
            int index = 0;

            foreach (var r in results)
            {
                ///download the img
                WebClient client = new WebClient();
                var img = client.DownloadData(new Uri(r["coverImage"]["medium"].ToString()));

                ///convert the img to a bitmap
                Stream stream = new MemoryStream(img);
                var image = new Avalonia.Media.Imaging.Bitmap(stream);

                ///add the img to the list
                imglist.Add(image);

                ///create the item
                var item = new MyItem
                {
                    Text = r["title"]["romaji"].ToString(),
                    Tags = new[] { r.ToString() },
                    ImageBitmap = imglist[index]
                };

                items.Add(item);
                index++;
            }

            //update the Ui of the listbox with the results of the research and the img 
            _ = Dispatcher.UIThread.InvokeAsync(() =>
            {
                mSearchResultsListBox.Items = null;
                mSearchResultsListBox.Items = items;
            });

        }

        private async void onSearchResulteSelectionChangedListBox(object sender, SelectionChangedEventArgs e)
        {
            /// if nothing is selected, return
            if (e.AddedItems.Count <= 0) return;
            var selectedResult = e.AddedItems[0];
            if (selectedResult == null) return;

            // get the selected item
            MyItem selectedItem = (MyItem)mSearchResultsListBox.SelectedItem;
            
            // get the tags
            string[] Tags = selectedItem.Tags;
            string Tag = Tags[0];

            // get the id
            string Id = JObject.Parse(Tag)["id"].ToString();

            // get the data from the api
            Uri url = new Uri($"https://find-my-anime.dtimur.de/api?id={Id}&provider=Anilist&includeAdult=true&collectionConsent=false\";");
            JObject response = await WebDb.CallApiJson(url);

            // add the data to the tags
            Array.Resize(ref Tags, selectedItem.Tags.Length + 1);
            selectedItem.Tags[selectedItem.Tags.Length - 1] = response.ToString();

            // get the AniDB id if it exist, else throw an exception
            try
            {
                string AniDBId = response["data"][0]["providerMapping"]["AniDB"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"AniDB id not found, ${ex}");
            }

            //errors I need to fix
            AnimeInfo animeInfo = new AnimeInfo();

            animeInfo.DisplayResults(animeInfo, selectedItem);

        }

    }
}
