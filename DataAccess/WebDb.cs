using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Net;
using MonoTorrent.Client;
using MonoTorrent;
using System.Linq;
using System.Threading;

namespace Avalonia_RandomAnimeTorrentApp.DataAccess
{
    public static class WebDb
    {
        /// <summary>
        /// one instance of HttpClient for all the application
        /// </summary>
        private static readonly HttpClient Client = new();

        /// <summary>
        /// call a Json request and return the response as a JObject
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<JObject> CallApiJson(Uri url)
        {
            HttpResponseMessage response = await Client.GetAsync(url).ConfigureAwait(true);
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject jsonResponse = JObject.Parse("{\"data\":" + responseBody + "}");
            return jsonResponse;
        }

        /// <summary>
        /// call a request to download an image and return it as a Bitmap
        /// </summary>
        /// <param name="url">image link, example : http://example.com/image.png</param>
        /// <returns></returns>
        public static async Task<Avalonia.Media.Imaging.Bitmap> CallApiBitmap(Uri url)
        {
            var response = await Client.GetAsync(url);
            var stream = await response.Content.ReadAsStreamAsync();
            var image = new Avalonia.Media.Imaging.Bitmap(stream);
            return image;
        }

        /// <summary>
        /// Download the torrent as file ( Current Directoryand / workspaceTorrent) and return it as a Stream
        /// </summary>
        /// <param name="uri"> uri to the .torrent</param>
        /// <param name="name"> name of the torrent, else will be "Torrent"</param>
        /// <param name="cToken"> Cancellation token, optional, else will be created but not returned</param>
        /// <returns></returns>
        public static async Task<Stream> Torrenting(Uri uri, String name = "Torrent", CancellationToken cToken = new())
        {
            ClientEngine engine = new();

            //define the workspace
            string? workspace = Path.Combine(Environment.CurrentDirectory, @"workspaceTorrent");

            // if the workspace doesn't exist, create it
            if (!Directory.Exists(workspace)) { Directory.CreateDirectory(workspace); }

            //download and load the torrent
            Torrent torrent = await Torrent.LoadAsync(Client, uri, Path.Combine(workspace, name + ".torrent"));

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
