using RestSharp;
using ShredClasses;
using System.Collections.Generic;
using System.Data.Common;
using TenmoClient.Helpers;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;
      
        public TenmoApiService(string apiUrl) : base(apiUrl) { }

        public int GetAccountById(int id)
        {
            IRestClient clint = TenmoApiService.client;
            RestRequest request = new RestRequest($"balance/account/{id}");
            IRestResponse response = client.Get(request);
            return int.Parse(response.Content);
        }
        public decimal  getBalanceById (int id)
        {
            IRestClient clint = TenmoApiService.client;
            RestRequest request = new RestRequest($"balance/{id}");
<<<<<<< HEAD
            IRestResponse response = client.Get(request);
            return decimal.Parse(response.Content);
=======
         
            IRestResponse response = client.Get(request);
            return decimal.Parse(response.Content);

>>>>>>> 7ae2a80d37bff2b6e9b986ca804e3648f1508539
        }

        public List<Transfer> GetAllTransfersForUser()
        {
            RestRequest request = new RestRequest($"transfer/user/{UserId}");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            return response.Data;
        }

<<<<<<< HEAD
        public List<Transfer> GetPendingTransfersForUser()
        {
            RestRequest request = new RestRequest($"transfer/pending/{UserId}");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            return response.Data;
        }

=======
>>>>>>> 7ae2a80d37bff2b6e9b986ca804e3648f1508539
        public List<User> GetAllUsers()
        {
            RestRequest request = new RestRequest($"user");
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            if (!response.IsSuccessful)
            {
                // request failed.
            }

            return response.Data;
        }

        public void TransferPay(int senderId, int targetId, decimal amount, bool isThisSend)
        {
<<<<<<< HEAD
            //int senderId, int targetId, int amount
            RestRequest request = new RestRequest($"transfer/pay");
            request.AddJsonBody( new { senderId = senderId, taregetId = targetId, amount = amount });
            IRestResponse  response = client.Post(request);
            var ppp = "ddfdf";
            if (!response.IsSuccessful)
            {
                // request failed.
            }
        }

        public User GetUserByAccountId(int id)
        {
            RestRequest request = new RestRequest($"user/account/{id}");
            IRestResponse<User> response = client.Get<User>(request);

            return response.Data;

        }

=======
            
            RestRequest request = new RestRequest($"transfer/SendMoney");
            request.AddJsonBody(new transfere_request(senderId,  targetId,  amount, isThisSend));
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Post(request);
            //if (!response.IsSuccessful)
            //{
            //    return BadRequest()// request failed.
            //}
        }

        public void ChangeTransferStatus(int _transid, int _appRej)
        {
            RestRequest request = new RestRequest($"transfer/AppRej");
            request.AddJsonBody(new TransferAppRej(_transid,_appRej));
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Post(request);
>>>>>>> 7ae2a80d37bff2b6e9b986ca804e3648f1508539

        }

        // Add methods to call api here...
        // TODO: 3. As an authenticated user of the system, I need to be able to see my Account Balance.

        // TODO: 4. As an authenticated user of the system, I need to be able to *send* a transfer of a specific amount of TE Bucks to a registered user.
        //  1. I should be able to choose from a list of users to send TE Bucks to.
        //  2. A transfer should include the User IDs of the from and to users and the amount of TE Bucks.
        //  3. A Sending Transfer has an initial status of *Approved*.
        //  4. The receiver's account balance is increased by the amount of the transfer.
        //  5. The sender's account balance is decreased by the amount of the transfer.
        //  6. I must not be allowed to send money to myself.
        //  7. I can't send more TE Bucks than I have in my account.
        //  8. I can't send a zero or negative amount.

        // TODO:  5. As an authenticated user of the system, I need to be able to see transfers I have sent or received.

    }
}
