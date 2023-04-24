using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Timers;
using Newtonsoft.Json.Linq;
using System.Linq;
using Avalonia.Threading;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Avalonia.Media.Imaging;
using System.Net;
using Avalonia_RandomAnimeTorrentApp.Models;
using Avalonia_RandomAnimeTorrentApp.Views;

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

            mSearchResultsListBox = this.FindControl<ListBox>("SearchResultsListBox") ?? throw new Exception("SearchResultsListBox not found");
            mSearchTextBox = this.FindControl<TextBox>("SearchTextBox") ?? throw new Exception("SearchTextBox not found");
            mGridSearchResultsListBox = this.FindControl<Grid>("GridSearchResultsListBox") ?? throw new Exception("GridSearchResultsListBox not found");
            mSearchTextBox.Focus();
            mSearchTextBox.AddHandler(TextInputEvent, TextBoxSearchQuerieUpdate, RoutingStrategies.Tunnel);

            timer = new System.Timers.Timer(500);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            timer.Stop();
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            mGridSearchResultsListBox.IsVisible = false;
        }

        private void TextBoxGotFocus(object sender, GotFocusEventArgs e)
        {
            mGridSearchResultsListBox.IsVisible = true;
        }

        private void TextBoxSearchQuerieUpdate(object sender, TextInputEventArgs e)
        {
            timer.Stop();
            timer.Start();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            TextBoxSearch(source, e);
        }

        private async Task<JObject> CallApiGraphQl(string Querie, string Varibles, Uri Url)
        {
            var requestObject = new GraphQLRequest
            {
                Query = Querie,
                Variables = Varibles
            };

            var client = new HttpClient();
            GraphQLHttpClient graphQLClient = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = Url }, new NewtonsoftJsonSerializer(), client);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = Url,
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestObject), Encoding.UTF8, "application/json")
            };

            //var response = await client.SendAsync(request);
            GraphQLResponse<dynamic> response = await graphQLClient.SendQueryAsync<dynamic>(requestObject);

            if (response.Errors != null)
            {
                throw new Exception("fetching anilist api failed");
            }
            JObject responseJson = response.Data;
            //JObject jsonResult = JObject.Parse(responseJson.ToString());


            return responseJson;
        }

        private async Task<JObject> CallApiJson(Uri url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject jsonResponse = JObject.Parse("{\"data\":" + responseBody + "}");
            return jsonResponse;
        }

        private async void TextBoxSearch(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            String recherche = mSearchTextBox.Text;
            List<String> ResultList = new List<String>();

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

            var variables = @"
            {""search"": """ + recherche + @"""}";

            Uri url = new Uri("https://graphql.anilist.co");

            JObject ApiResponse = await CallApiGraphQl(query, variables, url) ?? throw new Exception("Anlist api return null");

            var results = ApiResponse["Page"]["media"].ToList();

            List<Bitmap> imglist = new List<Bitmap>();

            int index = 0;

            List<MyItem> items = new List<MyItem>();

            foreach (var r in results)
            {
                WebClient client = new WebClient();

                var img = client.DownloadData(new Uri(r["coverImage"]["medium"].ToString()));
                Stream stream = new MemoryStream(img);
                var image = new Avalonia.Media.Imaging.Bitmap(stream);
                imglist.Add(image);


                var item = new MyItem
                {
                    Text = r["title"]["romaji"].ToString(),
                    Tags = new[] { r.ToString() },
                    ImageBitmap = imglist[index]


                    /*new Avalonia.Media.Imaging.Bitmap(new MemoryStream(new WebClient().DownloadDataAsync(new Uri(r["coverImage"]["large"].ToString()).AbsolutePath))*/
                };
                items.Add(item);
                index++;
            }
            _ = Dispatcher.UIThread.InvokeAsync(() =>
            {
                mSearchResultsListBox.Items = null;
                mSearchResultsListBox.Items = items;
            });

        }

        private async void onSearchResulteSelectionChangedListBox(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;
            var selectedResult = e.AddedItems[0];
            if (selectedResult == null) return;

            MyItem selectedItem = (MyItem)mSearchResultsListBox.SelectedItem;
            string[] Tags = selectedItem.Tags;
            string Tag = Tags[0];
            string Id = JObject.Parse(Tag)["id"].ToString();
            Uri url = new Uri($"https://find-my-anime.dtimur.de/api?id={Id}&provider=Anilist&includeAdult=true&collectionConsent=false\";");

            JObject response = await CallApiJson(url);

            Array.Resize(ref Tags, selectedItem.Tags.Length + 1);
            selectedItem.Tags[selectedItem.Tags.Length - 1] = response.ToString();

            try
            {
                string AniDBId = response["data"][0]["providerMapping"]["AniDB"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"AniDB id not found, ${ex}");
            }

            AnimeInfo animeInfo = new AnimeInfo();

            animeInfo.DisplayResults(animeInfo, selectedItem);

        }

    }
}
