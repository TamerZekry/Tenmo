using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
namespace shared
{
    public class htua
    {
        public static bool IsAuthrizedUser(HttpContext httpContext, int _userId)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            var claim = identity.Claims.ToList();
            var USER_ID = claim[0].Value;
            //var USER_NAME = claim[1].Value;
            return USER_ID == _userId.ToString();
        }
    }
}
