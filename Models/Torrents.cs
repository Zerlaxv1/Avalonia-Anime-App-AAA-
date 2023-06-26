using System;

namespace Avalonia_RandomAnimeTorrentApp.Models
{
    public class Torrents
    {
        /// <summary>
        /// number of seeders
        /// </summary>
        public int Seeders { get; set; }
        /// <summary>
        /// number of leechers
        /// </summary>
        public int Leechers { get; set; }
        /// <summary>
        /// torrent url
        /// </summary>
        public required String TorrentUrl { get; set; }
        /// <summary>
        /// title of the torrent
        /// </summary>
        public required String Title { get; set; }

    }
}
