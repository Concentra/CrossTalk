using System.Net.Http;
using System.Threading.Tasks;

namespace Crosstalk.Common.Client
{
    public interface IApiClient
    {
        Task<TModel> GetAsync<TModel>(string url);
        Task<TModel> PostAsync<TModel>(string url, TModel data);
        TModel Get<TModel>(string url);
        HttpResponseMessage Post<TModel>(string url, TModel data);

    }
}
