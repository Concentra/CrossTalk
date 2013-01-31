using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Crosstalk.Common.Exceptions;

namespace Crosstalk.Common.Client
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;
        protected HttpClient Client { get { return this._client; } }

        public ApiClient(string endpoint)
        {
            this._client = new HttpClient()
                {
                    BaseAddress = new Uri(endpoint)
                };
            this._client.DefaultRequestHeaders.Add("Accept", "application/javascript");
        }

        public async Task<TModel> GetAsync<TModel>(string url)
        {
            return await this._client.GetAsync(url).ContinueWith((t) =>
                {
                    this.EnsureSuccessStatusCode(t.Result);
                    return t.Result.Content.ReadAsAsync<TModel>().Result;
                });
        }
        
        public TModel Get<TModel>(string url)
        {
            var result = this._client.GetAsync(url).Result;
            this.EnsureSuccessStatusCode(result);
            return result.Content.ReadAsAsync<TModel>().Result;
        }

        public async Task<TModel> PostAsync<TModel>(string url, TModel data)
        {
            return await this._client.PostAsJsonAsync(url, data).ContinueWith((t) =>
                {
                    this.EnsureSuccessStatusCode(t.Result);
                    return t.Result.Content.ReadAsAsync<TModel>().Result;
                });
        }

        public HttpResponseMessage Post<TModel>(string url, TModel data)
        {
            var req = this._client.PostAsJsonAsync(url, data);
            this.EnsureSuccessStatusCode(req.Result);
            return req.Result;
        }

        private void EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new HttpNotFoundException(response.RequestMessage.RequestUri.AbsoluteUri);
                case HttpStatusCode.InternalServerError:
                    throw new HttpInternalServerErrorException(response.Content.ReadAsStringAsync().Result);
            }
        }

    }
}
