using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using static GraphQL.Validation.Rules.OverlappingFieldsCanBeMerged;
using System;
using Avalonia_RandomAnimeTorrentApp.Models;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using Avalonia_RandomAnimeTorrentApp.DataAccess;
using Avalonia.Media.Immutable;
using System.Drawing;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    internal partial class AnimeInfoViewModel : ObservableObject
    {
        [ObservableProperty]
        public string animeInfoDescriptionTextBlock = "TwT2";

        [ObservableProperty]
        public string animeInfoTitleLabel = "Title";

        [ObservableProperty]
        public Avalonia.Media.Imaging.Bitmap animeInfoImageBitmap;

        public AnimeInfoViewModel()
        {
            WeakReferenceMessenger.Default.Register<MyItem>(this, (r, m) =>
            {
                OnMyMessageReceived(m);
            });
        }

        private async void OnMyMessageReceived(MyItem message)
        {
            //test
            //AnimeInfoTitleLabel = string.Empty;

            //transform the string into a json object
            JObject requestFindMyAnime = JObject.Parse(message.Tags[1]);
            JObject requestAnilist = JObject.Parse(message.Tags[0]);

            //request image avec webdb
            Uri imgUri = new Uri(requestAnilist["coverImage"]["large"].ToString());
            Avalonia.Media.Imaging.Bitmap img = WebDb.CallApiBitmap(imgUri).Result;

            

            //changer l'ui
            AnimeInfoDescriptionTextBlock = requestFindMyAnime["data"][0]["description"].ToString();
            AnimeInfoTitleLabel = requestFindMyAnime["data"][0]["title"].ToString();
            AnimeInfoImageBitmap = img;
        }
    }
}
