using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shared;
using ShredClasses;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Models;


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

        //[HttpGet("pending/{transferId}")]
        //public ActionResult<IEnumerable<Transfer>> GetPendingTransfers(int userId)
        //{
        //    if (htua.IsAuthrizedUser(HttpContext, userId))
        //    {
        //        return _transferDao.GetPendingTransfers(userId);
        //    }

        //    return Forbid();
        //}

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<Transfer>> GetTransfersByUser(int userId)
        {
            if (htua.IsAuthrizedUser(HttpContext, userId))
            {
                return _transferDao.GetTransfersForUser(userId);
            }
            return Forbid();
        }


        [HttpPost("SendMoney")]
        public ActionResult PostTransfer(transfere_request transfere)
        {
            if (transfere.sender_Id == transfere.target_Id || transfere._amount <= 0 || !htua.IsAuthrizedUser(HttpContext, transfere.sender_Id))
            {
                // hello fellow traveler! wondering why this is here?
                // If you bypass the client theres no check for sending to your self and it WILL take down the server if called.
                // If you put in a negitive amount you will also Crash the server atleast theres a check in the DB or you could steal money!!
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
            if (_transferDao.GetTransferById(transferApp.Trans_id).Status == "Pending" && htua.IsAuthrizedUser(HttpContext, transferApp.SenderId))
            {
                return _transferDao.ChangeTransferStatus(transferApp.Trans_id, transferApp.Action_id);
            }
            return Forbid();
        }

    }
}
