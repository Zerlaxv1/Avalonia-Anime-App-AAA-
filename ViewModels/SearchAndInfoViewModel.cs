﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Avalonia_RandomAnimeTorrentApp.DataAccess;
using Avalonia_RandomAnimeTorrentApp.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Threading;
using System.Timers;
using Avalonia.SimpleRouter;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    // TODO: I made this public instead of internal
    // TODO: There should be some comments on the purpose of the ViewModel
    public partial class SearchAndInfoViewModel : ViewModelBase
    {

        private HistoryRouter<ViewModelBase> router;

        #region Public Properties

        private readonly System.Timers.Timer timer;

        /// <summary>
        /// List of Search Results based on the inputed TextBox string
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> searchResults = new();

        [ObservableProperty]
        private int resultHeight = 355;

        /// <summary>
        /// Flag to determine if the GridListBox should be visible
        /// </summary>
        [ObservableProperty]
        private bool isGridListBoxVisible = false;

        /// <summary>
        /// Result of the search are stored here
        /// </summary>
        [ObservableProperty]
        private List<MyItem> searchItems = new();

        /// <summary>
        /// I don't know why this is a string... it should be an ObservableCollection<string> I would think
        /// </summary>
        [ObservableProperty]
        private string myItems;

        /// <summary>
        /// Some generic png file string name
        /// </summary>
        [ObservableProperty]
        private string imageUrl = "https://i.imgur.com/1Z1Z1Z1.png";

        #endregion Public Properties


        #region Public Full Properties

        /// <summary>
        /// The backing member property for the Item in the ListBox that is selected
        /// </summary>
        private MyItem selectedItem;

        /// <summary>
        /// The Item in the ListBox that is selected
        /// </summary>
        public MyItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;

                // Call the SelectedItem changed method when an item in the Listbox is selected
                SearchResulteSelectionChangedAsync(selectedItem);
            }
        }

        /// <summary>
        /// The member that represents the text in the Textbox
        /// </summary>
        private string searchText;

        /// <summary>
        /// The Observable Property attached to the Textbox Text property
        /// </summary>
        public string SearchText
        {
            // Return the current Search Textbox Text
            get { return searchText; }

            // Set the new TextBox Text value to the ViewModel Property
            set
            {
                searchText = value;

                // If the ListBox is not visible... Set it to visible, and the opposite
                // TODO: | FINISHED | This needs to be changed so that something makes it invisible...
                // TODO: IsGridListBoxVisible false when lost focus
                // I would have a transparent overlay that is behind the Textbox and ListBox...
                // so when a user clicks on it it makes the ListBox visibility disappear again
                //if (searchText != "") { IsGridListBoxVisible = true; } else { IsGridListBoxVisible = false; }

                // Call the TextBoxSearch method using the updated TextBox Text
                timer.Stop();
                timer.Start();
                
            }
        }

        

        #endregion


        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SearchAndInfoViewModel(HistoryRouter<ViewModelBase> router)
        {
            this.router = router;

            timer = new System.Timers.Timer(500);
            timer.Elapsed += TextBoxSearch;
            timer.AutoReset = false;
            timer.Stop();
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Called when an Item in the ListBox is selected
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <returns> A completed Task</returns>
        /// <exception cref="Exception"></exception>
        private async Task SearchResulteSelectionChangedAsync(MyItem selectedItem)
        {
            if (selectedItem == null)
            {
                return;
            }

            // get the tags
            string[] Tags = selectedItem.Tags;
            string Tag = Tags[0];

            // get the id
            string Id = JObject.Parse(Tag)["id"].ToString();

            // get the data from the api
            Uri url = new($"https://find-my-anime.dtimur.de/api?id={Id}&provider=Anilist&includeAdult=true&collectionConsent=false\";");
            JObject response = await WebDb.CallApiJson(url);

            /// todo: fix this
            // add the data to the tags
            Array.Resize(ref Tags, Tags.Length + 1);
            Tags[^1] = response.ToString();

            selectedItem.Tags = Tags;

            // get the AniDB id if it exist, else throw an exception
            try
            {
                string AniDBId = response["data"][0]["providerMapping"]["AniDB"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"AniDB id not found, ${ex}");
            }

            AnimeInfoViewModel avm = router.GoTo<AnimeInfoViewModel>();
            avm.DisplayShit(selectedItem);

        }

        /// <summary>
        /// TODO:  This needs to be figured out..
        /// Called when the Text in the TextBox changes.
        /// It does a Json Search for items that name match to the TextBox Text.
        /// It doesn't return anything but sets the ListBox Items based on the TextBox Text
        /// </summary>
        /// <param name="searchText">The text in the Textbox</param>
        /// <exception cref="Exception"></exception>
        private async void TextBoxSearch(object? sender, ElapsedEventArgs e)
        {

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
            {""search"": """ + searchText + @"""}";

            ///call the api
            JObject ApiResponse = await Anilist.CallApi(query, variables) ?? throw new Exception("Anlist api return null");

            ///get the results and filter it a little
            var results = ApiResponse["Page"]["media"].ToList();

            ///initialise some lists
            ///

            // TODO:  I didn't change the imglist because I don't think you have it set up yet
            List<Bitmap> imglist = new();
            List<MyItem> items = new();

            ///set the index to 0
            int index = 0;

            foreach (var r in results)
            {
                ///download the img
                WebClient client = new();
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

            _ = Dispatcher.UIThread.InvokeAsync(() =>
            {
                SearchItems = items;
                ResultHeight = items.Count * 71;


            });
        }

        #endregion
    }
}
