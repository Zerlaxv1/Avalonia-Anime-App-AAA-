using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LibVLCSharp.Shared;
using System.Diagnostics;
using System.IO;
using Avalonia.SimpleRouter;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    public partial class PlayerViewModel : ViewModelBase
    {
        [ObservableProperty] private MediaPlayer mediaPlayerView;

        [ObservableProperty] private bool isLoading = true;

        private readonly LibVLC? VLC = new();

        private HistoryRouter<ViewModelBase> router;

        public PlayerViewModel(HistoryRouter<ViewModelBase> router)
        {
            this.router = router;
            Core.Initialize();
        }

        public void playBack(Stream stream)
        {
            Media media = new Media(VLC, new StreamMediaInput(stream));

            MediaPlayerView = new MediaPlayer(media);

            if (MediaPlayerView.IsSeekable)
            {
                MediaPlayerView.Play();
                Debug.WriteLine(" Log : Video seekable");
                IsLoading = false;
                media.Dispose();
            }
            else
            {
                Debug.WriteLine("Error : Video not seekable rn, waiting for seekable event");
                MediaPlayerView.Play();

                MediaPlayerView.SeekableChanged += (s, e) =>
                {
                    Debug.WriteLine(" Log : Video now seekable");
                    IsLoading = false;
                    media.Dispose();
                };
            }
        }
    }
}