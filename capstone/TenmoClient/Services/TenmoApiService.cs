using RestSharp;
using ShredClasses;
using System;
using System.Collections.Generic;
using System.Threading;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;
        public int UserAccountId { get; private set; }

        public TenmoApiService(string apiUrl) : base(apiUrl) { }

        public int GetAccountIdByUserId(int id)
        {
            RestRequest request = new RestRequest($"balance/account/{id}");
            IRestResponse response = client.Get(request);
            return int.Parse(response.Content);
        }
        public decimal GetBalanceById(int id)
        {
            RestRequest request = new RestRequest($"balance/{id}");
            IRestResponse response = client.Get(request);
            return decimal.Parse(response.Content);
        }

        public List<Transfer> GetAllTransfersForUser()
        {
            RestRequest request = new RestRequest($"transfer/user/{UserId}");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            return response.Data;
        }

        public List<Transfer> GetPendingTransfersForUser()
        {
            RestRequest request = new RestRequest($"transfer/pending/{UserId}");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            return response.Data;
        }

        public List<User> GetAllUsers()
        {
            RestRequest request = new RestRequest($"user");
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            return response.Data;
        }

        public void TransferPay(int senderId, int targetId, decimal amount, bool isThisSend)
        {
            RestRequest request = new RestRequest($"transfer/SendMoney");
            request.AddJsonBody(new transfere_request(senderId, targetId, amount, isThisSend));
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Post(request);
        }

        public void ChangeTransferStatus(int _transid, int _appRej)
        {
            RestRequest request = new RestRequest($"transfer/AppRej");
            request.AddJsonBody(new TransferAppRej(_transid, _appRej, UserId));
            request.AddHeader("Content-Type", "application/json");
            IRestResponse<bool> response = client.Post<bool>(request);
            if (!response.Data)
            {
                Console.WriteLine("You don't have enough money to send");
                Thread.Sleep(2000);
            }
        }

        public string GetUsernameByAccountId(int accountId)
        {
            RestRequest request = new RestRequest($"user/username/account/{accountId}");
            IRestResponse<string> response = client.Get<string>(request);

            return response.Data;
        }

        public override ApiUser Login(LoginUser loginUser)
        {
            var apiUser = base.Login(loginUser);
            if(apiUser != null)
            {
                UserAccountId = GetAccountIdByUserId(UserId);
            }
            
            return apiUser;
        }
        public override void Logout()
        {
            UserAccountId = 0;
            base.Logout();
        }
    }
}
