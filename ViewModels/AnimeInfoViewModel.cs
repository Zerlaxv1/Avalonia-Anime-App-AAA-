﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Avalonia_RandomAnimeTorrentApp.Models;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using Avalonia_RandomAnimeTorrentApp.DataAccess;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    internal partial class AnimeInfoViewModel : ObservableObject
    {

        JObject requestFindMyAnime;
        JObject requestAnilist;

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
            requestFindMyAnime = JObject.Parse(message.Tags[1]);
            requestAnilist = JObject.Parse(message.Tags[0]);

            //request image avec webdb
            Uri imgUri = new Uri(requestAnilist["coverImage"]["extraLarge"].ToString());
            Avalonia.Media.Imaging.Bitmap img = await WebDb.CallApiBitmap(imgUri);

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

        [RelayCommand]
        public async void PlayButton()
        {
            int aniDBId = Convert.ToInt32(requestFindMyAnime["data"][0]["providerMapping"]["AniDB"]);

            Uri uri = new($"https://feed.animetosho.org/json?aids={aniDBId}");
            JObject response = await WebDb.CallApiJson(uri);

            List<JToken> values = response["data"].ToList();
            List<Torrents> torrents = new List<Torrents>();
            Torrents TorrentWithTheMostSeeders = null;

            foreach (var i in values)
            {
                
                var seeders = i["seeders"].ToString();
                var leechers = i["leechers"].ToString();

                if ( seeders == "" || seeders == "0") {continue;}

                Torrents tor = new()
                {
                    Seeders = Convert.ToInt32(seeders),
                    Leechers = Convert.ToInt32(leechers),
                    TorrentUrl = i["torrent_url"].ToString(),
                    Title = i["title"].ToString()
                };

                torrents.Add(tor);
                
                if (TorrentWithTheMostSeeders == null || tor.Seeders > TorrentWithTheMostSeeders.Seeders)
                {
                    TorrentWithTheMostSeeders = tor;
                }

            }

            CancellationToken cancelToken = new CancellationToken();

            Stream stream = await WebDb.Torrenting(new Uri(TorrentWithTheMostSeeders.TorrentUrl), "torrrent", cancelToken);

            WeakReferenceMessenger.Default.Send<Stream>(stream);
        }
    }
}
