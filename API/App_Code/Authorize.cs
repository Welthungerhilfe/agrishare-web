/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode)
 */
 
 using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Collections.Generic;
using Agrishare.Core.Entities;
using Agrishare.Core.Utils;

namespace Agrishare.API
{
    public class Authorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Authentication required");
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var allowedRoles = Roles.Split(',').Select(e => (Role)Enum.Parse(typeof(Role), e.Trim(), true));
            if (allowedRoles.Count() == 0)
                return true;

            if (actionContext.Request.Headers.TryGetValues("Authorization", out var authorizationValues))
            {
                var token = authorizationValues.First();
                var user = User.Find(AuthToken: token);
                if (user?.Roles.Intersect(allowedRoles).Count() > 0)
                {
                    actionContext.Request.Properties["CurrentUser"] = user;
                    return true;
                }
            }

            return false;
            
        }
    }
}