using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Crosstalk.Core.Handlers
{
    public class CORSHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Headers.Contains("Origin"))
            {
                if (HttpMethod.Options == request.Method)
                {
                    return Task.Factory.StartNew<HttpResponseMessage>(() =>
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.Add("Access-Control-Allow-Origin", request.Headers.GetValues("Origin").First());

                        var method = request.Headers.GetValues("Access-Control-Request-Method").FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(method))
                        {
                            response.Headers.Add("Access-Control-Allow-Methods", method);
                        }

                        var headers = string.Join(", ", request.Headers.GetValues("Access-Control-Request-Headers"));
                        if (!string.IsNullOrWhiteSpace(headers))
                        {
                            response.Headers.Add("Access-Control-Allow-Headers", headers);
                        }

                        return response;
                    }, cancellationToken);
                }
                return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>(t => {
                        var response = t.Result;
                        response.Headers.Add("Access-Control-Allow-Origin", "*");
                        return response;
                    });
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}