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
using Avalonia.Media;
using System.Globalization;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    internal partial class AnimeInfoViewModel : ObservableObject
    {
        [ObservableProperty]
        public string animeInfoDescriptionTextBlock;

        [ObservableProperty]
        public string animeInfoTitleLabel;

        [ObservableProperty]
        public Avalonia.Media.Imaging.Bitmap animeInfoImageBitmap;

        [ObservableProperty]
        public bool isPlayButtonVisible = false;

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

            //transform the string into a json object
            JObject requestFindMyAnime = JObject.Parse(message.Tags[1]);
            JObject requestAnilist = JObject.Parse(message.Tags[0]);

            //request image avec webdb
            Uri imgUri = new Uri(requestAnilist["coverImage"]["extraLarge"].ToString());
            Avalonia.Media.Imaging.Bitmap img = WebDb.CallApiBitmap(imgUri).Result;

            string Description = requestFindMyAnime["data"][0]["description"].ToString().Replace("<br>", "\n").Replace("<i>", "").Replace("</i>", "");
            
            /*
            FormattedText descriptionFormatted = new FormattedText(
                Description,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                15,
                Avalonia.Media.Brushes.Black
                );
            */

            //changer l'ui
            IsPlayButtonVisible = true;
            AnimeInfoDescriptionTextBlock = Description;
            AnimeInfoTitleLabel = requestFindMyAnime["data"][0]["title"].ToString();
            AnimeInfoImageBitmap = img;
        }
    }
}
