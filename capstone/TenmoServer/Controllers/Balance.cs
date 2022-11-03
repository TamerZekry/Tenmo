using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;

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
            return _userDao.GetUserBalanceById(id);
        }


    }
}
