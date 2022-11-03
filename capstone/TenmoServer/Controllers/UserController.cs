using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDao _userDao;

        public UserController(IUserDao userDao)
        {
            _userDao = userDao;
        }

        /// <summary>
        /// return the balance of a user given the user id   at endpoint  apiUrl/user/id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
     
        
        //[HttpGet("{balance/{id}}")]
        //public decimal GetUserBalance(int id)
        //{
        //    return _userDao.GetUserBalanceById(id);
        //}
        /// <summary>
        /// return a List of users at endpoint  apiUrl/user
        /// apiUrl = "https://localhost:44315/";
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public ActionResult<List<User>>  Get()
        {
            return _userDao.GetUsers(); 
        }
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            return _userDao.GetUser(id.ToString());
        }

    }
}
