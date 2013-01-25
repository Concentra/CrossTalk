using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Crosstalk.Core.Exceptions;

namespace Crosstalk.Core.Handlers
{
    public class ObjectNotFoundExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is ObjectNotFoundException)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, actionExecutedContext.Exception);
            }
        }
    }
}