using Avalonia.Controls;
using Avalonia_RandomAnimeTorrentApp.ViewModels;
using LibVLCSharp.Avalonia.Unofficial;
using System;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Avalonia_RandomAnimeTorrentApp.Views
{
    public partial class PlayerView : ReactiveUserControl<PlayerViewModel>
    {
        public VideoView mVideoView;

        public PlayerView()
        {
            InitializeComponent();
            mVideoView = this.Get<VideoView>("VideoViewer") ?? throw new Exception("VideoViewer not found");
            Loaded += Player_Opened;
        }

        public void Player_Opened(object sender, EventArgs e)
        {
            //TODO: not linked to ViewModel
            if (mVideoView.PlatformHandle != null && ViewModel!.MediaPlayerView != null)
            {
                mVideoView.MediaPlayer = ViewModel.MediaPlayerView;
                mVideoView.MediaPlayer.SetHandle(mVideoView.PlatformHandle);
            } else {
                Console.WriteLine("Error: VideoView or MediaPlayerView is null");
            }
        }
    }
}
