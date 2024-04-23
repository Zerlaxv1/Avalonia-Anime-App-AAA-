using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MonoTorrent;
using MonoTorrent.Client;

namespace Avalonia_RandomAnimeTorrentApp.Models
{
    public class Torrent
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
        public String TorrentUrl { get; set; }
        /// <summary>
        /// title of the torrent
        /// </summary>
        public String Title { get; set; }

        public Torrent(int seeders, int leechers, string torrentUrl, string title)
        {
            Seeders = seeders;
            Leechers = leechers;
            TorrentUrl = torrentUrl;
            Title = title;
        }

        /// <summary>
        /// Download the torrent as file ( Current Directoryand / workspaceTorrent) and return it as a Stream
        /// </summary>
        /// <param name="uri"> uri to the .torrent</param>
        /// <param name="name"> name of the torrent, else will be "Torrent"</param>
        /// <param name="cToken"> Cancellation token, optional, else will be created but not returned</param>
        /// <returns></returns>
        public async Task<Stream> ToStream(String name = "Torrent", CancellationToken cToken = new())
        {
            HttpClient client = new();
            ClientEngine engine = new();

            //define the workspace
            string? workspace = Path.Combine(Environment.CurrentDirectory, @"workspaceTorrent");

            // if the workspace doesn't exist, create it
            if (!Directory.Exists(workspace)) { Directory.CreateDirectory(workspace); }

            //download and load the torrent
            MonoTorrent.Torrent torrent = await MonoTorrent.Torrent.LoadAsync(client, new Uri(this.TorrentUrl), Path.Combine(workspace, name + ".torrent"));

            //add the torrent to the engine to make the manager
            TorrentManager manager = await engine.AddStreamingAsync(torrent, workspace);

            //start the download
            await manager.StartAsync();

            //wait for metadata because we need it, but in general we already have it
            if (!manager.HasMetadata) { await manager.WaitForMetadataAsync(cToken); }

            //get the largest file (so the video, it's temporary)
            ITorrentManagerFile largestFile = manager.Files.OrderByDescending(f => f.Length).First();

            //dont download the others files
            foreach (var file in manager.Files)
            {
                if (file != largestFile)
                {
                    await manager.SetFilePriorityAsync(file, Priority.DoNotDownload);
                }
            }

            System.Threading.Timer timer = new((e) =>
            {
                Debug.WriteLine("file: " + largestFile.Path +
                    ", progress: " + manager.Progress +
                    ", download speed: " + manager.Monitor.DownloadRate +
                    ", upload speed: " + manager.Monitor.UploadRate +
                    ", peers: " + manager.Peers.Available +
                    ", seeds: " + manager.Peers.Seeds +
                    ", leechers: " + manager.Peers.Leechs +
                    ", downloaded: " + manager.Monitor.DataBytesReceived);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));

            //create the stream
            Stream stream = await manager.StreamProvider.CreateStreamAsync(largestFile, false, cToken);

            return stream;
        }
    }
}
