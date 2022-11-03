using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.DAO;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {

        private readonly ITransferDao _transferDao;
        private readonly IUserDao _userDao;

        public TransferController(ITransferDao transferDao, IUserDao userDao)
        {
            _transferDao = transferDao;
            _userDao = userDao;
        }

        // TODO: GET List of past transactions from user
        [HttpGet("{userId}")]
        [Authorize]
        public ActionResult<List<Transfer>> GetTransfers(int userId)
        {
            var user =_userDao.GetUser(User.Identity.Name);
            if(user == null || userId != user.UserId) 
            {
                return Forbid();
            }
            return _transferDao.GetTransfersForUser(userId);
        }
        [HttpGet("pending/{transferId}")]
        public ActionResult<IEnumerable<Transfer>> GetPendingTransfers(int userId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<Transfer>> GetTransfersByUser(int userId)
        {
            throw new NotImplementedException();
        }
        [HttpGet("{transferId}")]
        public ActionResult<Transfer> GetTransferById(int transferId)
        {
            throw new NotImplementedException();
            return _transferDao.GetTransferById(transferId);
        }
        [HttpPost]

        public void PostTransferRequest(int senderId, int targetId, int amount)
        {
            throw new NotImplementedException();
        }

        public void PostTransfer(int senderId, int targetId, int amount)
        {
            throw new NotImplementedException();
        }
    }
}
