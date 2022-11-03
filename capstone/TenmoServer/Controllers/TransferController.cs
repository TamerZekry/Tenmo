using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.DAO;
using TenmoServer.Helpers;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {

        private readonly ITransferDao _transferDao;

        public TransferController( ITransferDao transferDao)
        {
            _transferDao = transferDao;
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
