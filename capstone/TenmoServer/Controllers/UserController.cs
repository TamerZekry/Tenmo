using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        // TODO: GET Specific user balance

        // GET: /<UserController>
        [HttpGet("{id}")]
        public decimal GetUserBalance(int id)
        {
            return new decimal();
        }


        // TODO: GET List of users

        // GET /<UserController>/5
        [HttpGet("")]
        public IEnumerable<string> Get()
        {
            return new List<string>() ;
        }
    }
}
