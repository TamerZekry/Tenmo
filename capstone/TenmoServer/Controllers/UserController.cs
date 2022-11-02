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
        //// TODO: GET Specific user balance

        //// GET: /<UserController>
        [HttpGet("{id}")]
        public decimal GetUserBalance(int id)
        {
            return _userDao.GetUserBalanceById(id);
        }


        // TODO: GET List of users

        //Get All registered users
        [HttpGet("")]
        public ActionResult<List<User>>  Get()
        {
            return _userDao.GetUsers(); ;
        }
    }
}
