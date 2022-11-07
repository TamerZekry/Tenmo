using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Extentions;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class BalanceController : ControllerBase
    {
        private readonly IUserDao _userDao;

        public BalanceController(IUserDao userDao)
        {
            _userDao = userDao;
        }

        /// <summary>
        /// return the balance of a user given the user id  at endpoint  apiUrl/balance/id
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<decimal> GetUserBalance(int id)
        {
            if (!this.CurrentUserIdEquals(id))
            {
                return Forbid();
            }
            return _userDao.GetUserBalanceById(id);
        }

        /// <summary>
        /// return Account ID by passing User ID
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet("account/{id}")]
        public ActionResult<int> GET(int id)
        {
            if (!this.CurrentUserIdEquals(id))
            {
                return Forbid();
            }
            return _userDao.GetAccountId(id);
        }
    }
}
