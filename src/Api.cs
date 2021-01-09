using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Sparx.Sdk.Models;

namespace Sparx.Sdk
{
    public class Api
    {
        private readonly string _domain;
        private readonly string _client;
        private readonly string _secret;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="domain">Domain of your instance</param>
        /// <param name="client">Client from personal account</param>
        /// <param name="secret">Secret from personal account</param>
        public Api(string domain, string client, string secret)
        {
            _domain = domain;
            _client = client;
            _secret = secret;
        }


        /// <summary>
        /// Asynchronous call to API
        /// </summary>
        /// <param name="method">API method (i.e. "/v1/tariff")</param>
        /// <param name="body">Contents of the HTTP request</param>
        /// <param name="requestType">Type of request (PUT, POST, GET, DELETE)</param>
        /// <returns>Response message</returns>
        public async Task<string> MakeRequestAsync(string method, string body = null, HttpMethod requestType = null)
        {
            if (method.StartsWith("/"))
                method = method.Remove(0, 1);

            var httpClient = await GetAuthenticatedClientAsync(_client, _secret);

            if (requestType == null) requestType = HttpMethod.Get;

            var request = new HttpRequestMessage(requestType, method);

            if (requestType == HttpMethod.Post || requestType == HttpMethod.Patch || requestType == HttpMethod.Put)
            {
                HttpContent httpContent = new StringContent(body, Encoding.UTF8, "application/json");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = httpContent;
            }

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostAsJsonAsync<TValue>(string requestUri, TValue value, CancellationToken cancellationToken)
        {
            var client = await GetAuthenticatedClientAsync(_client, _secret);
            var content = JsonContent.Create(value);
            var response = await client.PostAsync(requestUri, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }


        private async Task<HttpClient> GetAuthenticatedClientAsync(string clientId, string clientSecret)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri($"{_domain}/api/"),
            };

            var token = await RequestTokenAsync(client, clientId, clientSecret);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private async Task<string> RequestTokenAsync(HttpClient client, string clientId, string clientSecret)
        {
            HttpContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            });

            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var httpResponse = await client.PostAsync("connect/token", content);

            var tokenResult = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<TokenResponse>(tokenResult);

            if (!httpResponse.IsSuccessStatusCode)
                throw new Exception(response?.Error);

            return response?.AccessToken;
        }

        // Temporary is not used
        // private async Task<string> RequestTokenAsync(HttpClient client, string clientId, string clientSecret)
        // {
        //     var disco = await client.GetDiscoveryDocumentAsync();
        //
        //     if (disco.IsError)
        //         throw new Exception(disco.Error);
        //
        //     var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        //     {
        //         Address = disco.TokenEndpoint,
        //         ClientId = clientId,
        //         ClientSecret = clientSecret
        //     });
        //
        //     if (response.IsError)
        //         throw new Exception(response.Error);
        //
        //     return response.AccessToken;
        // }
    }
}