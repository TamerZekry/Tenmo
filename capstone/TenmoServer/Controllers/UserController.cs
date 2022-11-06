using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserDao _userDao;

        public UserController(IUserDao userDao)
        {
            _userDao = userDao;
        }

        /// <summary>
        /// return a List of users at endpoint  apiUrl/user
        /// apiUrl = "https://localhost:44315/";
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public ActionResult<List<User>> Get()
        {
            return _userDao.GetUsers();
        }

        [HttpGet("username/account/{accountId}")]
        public ActionResult<string> GetUsernameByAccountId(int accountId)
        {
            return _userDao.GetUsernameByAcount(accountId);
        }
    }
}
