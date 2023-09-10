using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LibVLCSharp.Shared;
using System.Diagnostics;
using System.IO;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    public partial class PlayerViewModel : ObservableObject
    {
        [ObservableProperty]
        public MediaPlayer mediaPlayerView;

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
                    Debug.WriteLine("Seekable");
                    MediaPlayerView.Play();
                    media.Dispose();
                } else
                {
                    Debug.WriteLine("Not seekable");
                }
            };

            MediaPlayerView.Play();
            media.Dispose();

            Debug.WriteLine(MediaPlayerView.IsSeekable);
         }
    }
}
