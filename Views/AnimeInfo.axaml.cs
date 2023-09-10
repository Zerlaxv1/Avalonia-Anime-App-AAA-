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
        /*
        #region private members

        private Image mimageInfo;
        private Button mplayButton;
        private Label mlabelTitleInfo;
        private TextBlock mtextBlockDescriptionInfo;

        #endregion
        */
        public AnimeInfo()
        {
            InitializeComponent();
            /*
            //some shit needed 
            mimageInfo = this.FindControl<Image>("imageInfo") ?? throw new Exception("ImageInfo not found");
            mplayButton = this.FindControl<Button>("playButton") ?? throw new Exception("PlayButton not found");
            mlabelTitleInfo = this.FindControl<Label>("labelTitleInfo") ?? throw new Exception("LabelTitleInfo not found");
            mtextBlockDescriptionInfo = this.FindControl<TextBlock>("textBlockDescriptionInfo") ?? throw new Exception("TextBlockDescriptionInfo not found");
            mlabelTitleInfo.Content = "Title2";
            */
            this.DataContext = new AnimeInfoViewModel();
        }
    }
}
