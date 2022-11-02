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
    public class TransferController : ControllerBase
    {






        // GET: <TransferController>/{id}


        // TODO: GET List of past transactions from user
        [HttpGet("{id}")]
        public IEnumerable<string> GetTransfers(int userId)
        {
            
            return new string[] { "value1", "value2" };
        }


        // TODO: GET Transfer details by transaction ID
        // GET <TransferController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // TODO: POST Transfer(int targetID, int senderID, decimal amountToSend)
        // POST <TransferController>
        [HttpPost("")]
        public void Post([FromBody] int targetID, [FromBody] int senderID, [FromBody] decimal amountToSend)
        {
        }

        //TODO: GET check if user can afford a transfer

        // TODO: Put Decrease user balance by transfer amount
        // PUT <TransferController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }


        // TODO: GET status of transfer
        // TODO: PUT approve or deny transfer



        // DELETE <TransferController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
