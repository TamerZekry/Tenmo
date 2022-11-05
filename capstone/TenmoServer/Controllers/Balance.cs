using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shared;
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
            if (!htua.IsAuthrizedUser(HttpContext, id))
            {
                return 0;
            }
            else
            {
                return _userDao.GetUserBalanceById(id);
            }
        }

        [HttpGet("account/{id}")]
        public int GET(int id)
        {
            return _userDao.GetAccountId(id);
        }
    }
}
