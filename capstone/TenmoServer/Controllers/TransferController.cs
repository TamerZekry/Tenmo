using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.DAO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        ITransferDao transferDao = new TransferDao();





        // GET: <TransferController>/{id}


        // TODO: GET List of past transactions from user
        [HttpGet("{id}")]
        public IEnumerable<Transfer> GetTransfers(int userId)
        {
            return transferDao.GetTransfersForUser(userId);
        }


        // TODO: GET Transfer details by transaction ID
        // GET <TransferController>/5
        [HttpGet("{id}")]
        public Transfer Get(int id)
        {
            return transferDao.GetTransferById(id);
        }

        // TODO: POST Transfer(int targetID, int senderID, decimal amountToSend)
        // POST <TransferController>
        [HttpPost("")]
        public void Transfer([FromBody] int targetID, [FromBody] int senderID, [FromBody] decimal amountToSend)
        {
            transferDao.SendTransfer(targetID, senderID, amountToSend);
        }

        // TODO: GET check if user can afford a transfer
        [HttpPost("")]
        public void CheckIfViableTransfer([FromBody] int targetID, [FromBody] int senderID, [FromBody] decimal amountToSend)
        {
            transferDao.SendTransfer(targetID, senderID, amountToSend);
        }


        // TODO: Put Decrease user balance by transfer amount
        // PUT <TransferController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }


        // TODO: GET status of transfer
        // TODO: PUT approve or deny transfer
    }
}
