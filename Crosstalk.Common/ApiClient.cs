using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Crosstalk.Common
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
        }
        
        public async Task<TModel> Get<TModel>(string url)
        {
            var result = this._client.GetAsync(url).Result;
            return result.IsSuccessStatusCode ? result.Content.ReadAsAsync<TModel>().Result : default(TModel);
        }

        public async Task<HttpResponseMessage> Post<TModel>(string url, TModel data)
        {
            return await this._client.PostAsJsonAsync(url, data);
        }

    }
}
