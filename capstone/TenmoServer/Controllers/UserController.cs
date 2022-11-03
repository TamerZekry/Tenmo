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
        [HttpGet("balance/{id}")]
        public decimal GetBalanceFromUser(int id)
        {
            throw new NotImplementedException();
            //Use bottom after merging with updated DAO branch
            //return _userDao.GetUserBalance(id);
        }


        [HttpGet]
        public ActionResult<List<User>> GetUsers()
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
