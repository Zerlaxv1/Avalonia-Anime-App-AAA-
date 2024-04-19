using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LibVLCSharp.Shared;
using System.Diagnostics;
using System.IO;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    public partial class PlayerViewModel : ObservableObject
    {
        [ObservableProperty] private MediaPlayer mediaPlayerView;

        [ObservableProperty] private bool isLoading = true;

        LibVLC? _libVLC = new();
        Media media;

        public PlayerViewModel()
        {
            WeakReferenceMessenger.Default.Register<Stream>(this, (r, m) =>
            {
                playBack(m);
            });
        }

         public void playBack(Stream stream)
         {
            media = new Media(_libVLC, new StreamMediaInput(stream));

            MediaPlayerView = new MediaPlayer(media);

            MediaPlayerView.SeekableChanged += (s,e) =>
            {
                if (MediaPlayerView.IsSeekable == true)
                {
                    Debug.WriteLine(" Log : Video seekable");
                    IsLoading = false;
                    MediaPlayerView.Play();
                    media.Dispose();
                } else
                {
                    Debug.WriteLine("Error : Video not seekable");
                }
            };

            MediaPlayerView.Play();
            media.Dispose();

            Debug.WriteLine(MediaPlayerView.IsSeekable);
         }
    }
}
