using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia_RandomAnimeTorrentApp.Models;
using System;
using Avalonia_RandomAnimeTorrentApp.ViewModels;
using System.Threading.Tasks;

namespace Avalonia_RandomAnimeTorrentApp.Views
{
    public partial class AnimeInfo : UserControl
    {

        #region private members

        private Image mimageInfo;
        private Button mplayButton;
        private Label mlabelTitleInfo;
        private TextBlock mtextBlockDescriptionInfo;

        #endregion

        public AnimeInfo()
        {
            InitializeComponent();

            //some shit needed 
            mimageInfo = this.FindControl<Image>("imageInfo") ?? throw new Exception("ImageInfo not found");
            mplayButton = this.FindControl<Button>("playButton") ?? throw new Exception("PlayButton not found");
            mlabelTitleInfo = this.FindControl<Label>("labelTitleInfo") ?? throw new Exception("LabelTitleInfo not found");
            mtextBlockDescriptionInfo = this.FindControl<TextBlock>("textBlockDescriptionInfo") ?? throw new Exception("TextBlockDescriptionInfo not found");
            mlabelTitleInfo.Content = "Title2";
            this.DataContext = new AnimeInfoViewModel();
            
            //to test it will not stay here
            DisplayResults2();
        }

        /// <summary>
        /// need to be fixed but in a near futur it will display the results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selected"></param>
        public async void DisplayResults(object sender, MyItem selected)
        {
            //selected.Tags[0]



            var animeInfoControl = sender as AnimeInfo;
            if (animeInfoControl == null)
            {
                // Traitement d'erreur
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                animeInfoControl.mlabelTitleInfo.Content = "uwu";
                (DataContext as AnimeInfoViewModel).AnimeInfoDescriptionTextBlock = "98729874648564";
                ((AnimeInfoViewModel)animeInfoControl.DataContext).AnimeInfoDescriptionTextBlock = "sdiufguisdgfyuisdgfuisdgfuysdgfyu";
                animeInfoControl.mtextBlockDescriptionInfo.Text = "owo2375574546";
            });

        }
        /// <summary>
        /// will be deleted
        /// </summary>
        public async void DisplayResults2()
        {
            //selected.Tags[0]
            await Task.Delay(2000);
            (DataContext as AnimeInfoViewModel).AnimeInfoDescriptionTextBlock = "sdiufguisdgfyuisdgfuisdgfuysdgfyu23";
            /*
            Dispatcher.UIThread.Post(() =>
            {
                mlabelTitleInfo.Content = "uwu23";
                mtextBlockDescriptionInfo.Text = "owo23";
            });
            */
        }

    }
}
