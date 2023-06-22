using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Net;

namespace Avalonia_RandomAnimeTorrentApp.DataAccess
{
    public static class WebDb
    {
        /// <summary>
        /// call a graphql request and return the response as a JObject
        /// </summary>
        /// <param name="Querie"></param>
        /// <param name="Varibles"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<JObject> CallApiGraphQl(string Querie, string Varibles, Uri Url)
        {
            var requestObject = new GraphQLRequest
            {
                Query = Querie,
                Variables = Varibles
            };

            var client = new HttpClient();
            GraphQLHttpClient graphQLClient = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = Url }, new NewtonsoftJsonSerializer(), client);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = Url,
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestObject), Encoding.UTF8, "application/json")
            };

            //var response = await client.SendAsync(request);
            GraphQLResponse<dynamic> response = await graphQLClient.SendQueryAsync<dynamic>(requestObject);

            if (response.Errors != null)
            {
                throw new Exception("fetching anilist api failed");
            }
            JObject responseJson = response.Data;
            //JObject jsonResult = JObject.Parse(responseJson.ToString());

            return responseJson;
        }

        /// <summary>
        /// call a Json request and return the response as a JObject
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<JObject> CallApiJson(Uri url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
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
            WebClient client = new();
            var responseData = client.DownloadData(url);
            Stream stream = new MemoryStream(responseData);
            var image = new Avalonia.Media.Imaging.Bitmap(stream);
            return image;
        }
    }
}
