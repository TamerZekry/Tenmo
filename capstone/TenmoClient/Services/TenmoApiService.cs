using RestSharp;
using System.Collections.Generic;
using TenmoClient.Models;


namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;
      
        public TenmoApiService(string apiUrl) : base(apiUrl) { }

         public decimal  getBalanceById (int id)
        {
            IRestClient clint = TenmoApiService.client;
            RestRequest request = new RestRequest($"user/{id}");
         //   request.AddObject(obj)
            IRestResponse response = client.Get(request);
            return decimal.Parse(response.Content);


        }
        public List<Transfer> GetAllTransfersForUser()
        {
            RestRequest request = new RestRequest($"transfer/{UserId}");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            if(!response.IsSuccessful)
            {
                // request failed.
            }

            return response.Data;
        }





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
