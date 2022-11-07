using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shared;
using shared.Models;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Extentions;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private readonly ITransferDao _transferDao;
        private readonly IUserDao _userDao;

        public TransferController(ITransferDao transferDao, IUserDao userDao)
        {
            _transferDao = transferDao;
            _userDao = userDao;
        }

        [HttpGet("pending/{userId}")]
        public ActionResult<IEnumerable<Transfer>> GetPendingTransfers(int userId)
        {
            if (this.CurrentUserIdEquals(userId))
            {
                return _transferDao.GetPendingTransfers(userId);
            }

            return Forbid();
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<Transfer>> GetTransfersByUser(int userId)
        {
            if (this.CurrentUserIdEquals(userId))
            {
                return _transferDao.GetTransfersForUser(userId);
            }
            return Forbid();
        }

        [HttpPost("SendMoney")]
        public ActionResult PostTransfer(TransferRequest transfere)
        {
            if (transfere.sender_Id == transfere.target_Id || transfere._amount <= 0 || !this.CurrentUserIdEquals(transfere.sender_Id))
            {
                // hello fellow traveler! wondering why this is here?
                // If you bypass the client theres no check for sending to your self and it WILL take down the server if called.
                // If you put in a negetive amount you will also Crash the server at least theres a check in the DB or you could steal money!!
                return Forbid();
            }
            decimal balance = _userDao.GetUserBalanceById(transfere.sender_Id);
            if (transfere.IsThisASend && balance < transfere._amount)
            {
                // A user cannot send more money than they have.
                return Forbid();
            }

            _transferDao.SendTransfer(transfere.sender_Id, transfere.target_Id, transfere._amount, transfere.IsThisASend);
            return Ok();
        }

        [HttpPost("AppRej")]
        public ActionResult<bool> ApproveReject(TransferAppRej transferApp)
        {
            // if you bypass the client you can set any of your own transfers to any state.
            // we need to get the transfer and check to make sure its actually still pending here still before doing anything.
            // if not you should be forbiden from altering its state.
            if (_transferDao.GetTransferById(transferApp.Trans_id).Status == "Pending" && this.CurrentUserIdEquals(transferApp.SenderId))
            {
                return _transferDao.ChangeTransferStatus(transferApp.Trans_id, transferApp.Action_id);
            }
            return Forbid();
        }
    }
}
