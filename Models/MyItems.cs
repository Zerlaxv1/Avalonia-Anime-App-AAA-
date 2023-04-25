using System;

namespace Avalonia_RandomAnimeTorrentApp.Models
{
    public class MyItem
    {
        /// <summary>
        /// The Ttile of the item
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The description of the item, the links to the torrent and the image etc...
        /// </summary>
        public string[] Tags { get; set; }
        /// <summary>
        /// The image of the item that will be displayed
        /// </summary>
        public Avalonia.Media.Imaging.Bitmap ImageBitmap { get; set; }

    }
}
