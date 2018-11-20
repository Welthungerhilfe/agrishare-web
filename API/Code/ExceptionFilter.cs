/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */

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