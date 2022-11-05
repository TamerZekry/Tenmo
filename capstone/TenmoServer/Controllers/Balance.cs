using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShredClasses;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly IUserDao _userDao;

        public BalanceController(IUserDao userDao)
        {
            _userDao = userDao;
        }

        /// <summary>
        /// return the balance of a user given the user id   at endpoint  apiUrl/balance/id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public decimal GetUserBalance(int id)
        {

            if (!htua.IsAuthrizedUser(HttpContext, id))
            {
                return 0;
            }
            else
            {
            return _userDao.GetUserBalanceById(id);

            }
           
            
            
            
            //// var identity = HttpContext.User.Identity as ClaimsIdentity;
            //var claim = identity.Claims.ToList<Claim>();
            //var USER_ID = claim[0].Value;
            //var USER_NAME = claim[1].Value;




        }

        [HttpGet("account/{id}")]
        public int GET(int id)
        {
            return _userDao.GetAccountId(id);
        }
    }
}
