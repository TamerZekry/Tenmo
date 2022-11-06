using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
 

namespace TenmoServer.Security
{
    public class MyAuthorizationAttribute : AuthorizeAttribute
{
        protected  bool AuthorizeCore(HttpContext httpContext,string _userName)
        {
            if (httpContext.User.Identity.Name.Equals(_userName))
            {
                return true;
            }
            return false;
        }
         
        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    //Handle unauthorized logic
        //}
    }

}
