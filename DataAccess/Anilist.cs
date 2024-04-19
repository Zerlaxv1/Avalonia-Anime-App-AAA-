using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Avalonia_RandomAnimeTorrentApp.DataAccess
{
    public static class Anilist
    {
        /// <summary>
        /// Http client to make the request
        /// </summary>
        private static readonly HttpClient Client = new();

        /// <summary>
        /// call a graphql request and return the response as a JObject
        /// </summary>
        /// <param name="querie"></param>
        /// <param name="varibles"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<JObject> CallApi(string querie, string varibles)
        {
            Uri url = new Uri("https://graphql.anilist.co");

            var requestObject = new GraphQLRequest
            {
                Query = querie,
                Variables = varibles
            };
            GraphQLHttpClient graphQlClient = new GraphQLHttpClient(new GraphQLHttpClientOptions { EndPoint = url }, new NewtonsoftJsonSerializer(), Client);

            /*var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = url,
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestObject), Encoding.UTF8, "application/json")
            };*/

            GraphQLResponse<dynamic> response = await graphQlClient.SendQueryAsync<dynamic>(requestObject);

            if (response.Errors != null)
            {
                throw new Exception("Error : fetching Anilist api failed : " + response.Errors[0].Message);
            }

            return response.Data;
        }
    }
}
