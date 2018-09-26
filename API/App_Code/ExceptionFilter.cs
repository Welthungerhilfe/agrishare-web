using Agrishare.Core.Entities;
using System.Web.Http.Filters;

namespace Agrishare.API
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            var path = actionExecutedContext.Request.RequestUri.LocalPath;
            Log.Error(path, exception);
        }
    }
}