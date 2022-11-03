﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.DAO;
using TenmoServer.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public IEnumerable<Transfer> GetPendingTransfers(int userId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("user/{userId}")]
        public IEnumerable<Transfer> GetTransfersByUser(int userId)
        {
            throw new NotImplementedException();
        }
        [HttpGet("{transferId}")]
        public IEnumerable<Transfer> GetTransferById(int transferId)
        {
            throw new NotImplementedException();
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


        /*

        // GET: <TransferController>/{id}


        // TODO: GET List of past transactions from user
        [HttpGet("{id}")]
        public IEnumerable<Transfer> GetTransfers(int userId)
        {
            return _transferDao.GetTransfersForUser(userId);
        }


        // TODO: GET Transfer details by transaction ID
        // GET <TransferController>/5
        [HttpGet("{id}")]
        public Transfer Get(int id)
        {
            return _transferDao.GetTransferById(id);
        }

        // TODO: POST Transfer(int targetID, int senderID, decimal amountToSend)
        // POST <TransferController>
        [HttpPost("transfer/")]
        public void TryTransfer([FromBody] int targetID, [FromBody] int senderID, [FromBody] decimal amountToSend)
        {
            if (CanTransfer(targetID, senderID, amountToSend)) {
                Transfer(targetID, senderID, amountToSend);
            }
        }

        private bool CanTransfer(int targetID, int senderID, decimal amountToSend)
        {
            if (!TransferChecker.CheckIfCanAfford() || !TransferChecker.CheckIfNegative() || !TransferChecker.CheckIfValidReciever() || !TransferChecker.CheckIfValidSender())
                return false;
            return true;
        }

        public void Transfer([FromBody] int targetID, [FromBody] int senderID, [FromBody] decimal amountToSend)
        {
            _transferDao.SendTransfer(targetID, senderID, amountToSend);
        }

        // TODO: GET check if user can afford a transfer
        [HttpPost("")]
        public void CheckIfViableTransfer([FromBody] int targetID, [FromBody] int senderID, [FromBody] decimal amountToSend)
        {
            _transferDao.SendTransfer(targetID, senderID, amountToSend);
        }


        // TODO: Put Decrease user balance by transfer amount
        // PUT <TransferController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }


        // TODO: GET status of transfer
        // TODO: PUT approve or deny transfer
        */
    }
}
