using System;
using System.Collections.Generic;
using System.Linq;
using TenmoClient.Helpers;
using TenmoClient.Models;
using TenmoClient.Services;

namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;

        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
            
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                // View logged in user balance
                console.Pause("Your current account balance is :" + tenmoApiService.GetBalanceById(tenmoApiService.UserId).ToString("C"));
            }

            if (menuSelection == 2)
            {
                var transfersList = tenmoApiService.GetAllTransfersForUser();
                var accountIdToUsernameLookup = CreateAccountToUsernameLookup(transfersList);

                // View your past transfers
                console.PrintViewTransfersMenu(
                    transfers: transfersList,
                    accountId: tenmoApiService.UserAccountId,
                    userNameLookup: accountIdToUsernameLookup);

                if(transfersList.Count > 0)
                {
                    Transfer selectedTransfer = console.PromptForTransfer(transfersList, "Please enter transfer ID to view details (0 to cancel)");

                    if (selectedTransfer != null)
                    {
                        console.PrintTransferDetails(selectedTransfer, accountIdToUsernameLookup);
                    }
                }
                else
                {
                    console.Pause("No transfers. (press any key)");
                }
                

            }

            if (menuSelection == 3) // View pending transfer
            {
                var pendingTransfers = (from x in tenmoApiService.GetAllTransfersForUser() where (x.Status == "Pending" && x.From == tenmoApiService.UserAccountId) select x).ToList<Transfer>();
                var userNameLookup = CreateAccountToUsernameLookup(pendingTransfers);

                console.PrintPendingTransferMenu(pendingTransfers, userNameLookup);

                // CHOOSE THE TRANSFER

                if (pendingTransfers.Count > 0)
                {
                    Transfer transferToBechanged = console.PromptForTransfer(pendingTransfers, "Please enter transfer ID to approve/reject (0 to cancel)");
                    if(transferToBechanged == null)
                    {
                        return true;
                    }
                    console.PrintApproveOrReject();
                    // CHANGE THE STATUS either approved it or rejected 
                    int actionToTake = console.PromptForInteger("Please choose number", 0, 2, 0);

                    if (actionToTake == 0)
                    {
                        return true;
                    }

                    tenmoApiService.ChangeTransferStatus(transferToBechanged.Id, actionToTake);
                }
                else
                {
                    console.Pause("No pending transfers. (press any key)");
                }
            }

            if (menuSelection == 4) // Send Money
            {
                var userList = tenmoApiService.GetAllUsers();
                PrintingUserList.PrintUsers(userList);

            enterID:
                int userIdtoSendMonyTo = console.PromptForInteger("Enter user ID to send to", 0);
                if (userIdtoSendMonyTo == 0)
                {
                    return true;
                }

                if (TheChecker.AreEqual(userIdtoSendMonyTo, tenmoApiService.UserId))
                {
                    console.PrintError("You can't send money to your self");
                    goto enterID;
                }
                if ((userList.FirstOrDefault(x => x.UserId == userIdtoSendMonyTo)) == null)
                {
                    console.PrintError("Please Enter a valid User id");
                    goto enterID;
                }
            enterMoney:
                decimal amountOfMoneytoBeSend = console.PromptForDecimal("Enter amount of money", 0.00m);
                if (amountOfMoneytoBeSend == 0)
                {
                    return true;
                }

                if (TheChecker.LeftGreaterthe(0, amountOfMoneytoBeSend) ||
                    amountOfMoneytoBeSend > tenmoApiService.GetBalanceById(tenmoApiService.UserId)
                    )
                {
                    console.PrintError("You can't send this amount money");
                    goto enterMoney;
                }
                //call   transfer send money 
                tenmoApiService.TransferPay(tenmoApiService.UserId, userIdtoSendMonyTo, amountOfMoneytoBeSend, true);


                console.Pause();
            }

            if (menuSelection == 5) // Request money
            {
                var users = tenmoApiService.GetAllUsers();
                PrintingUserList.PrintUsers(users);
            enterID2:
                int userIdtoToRequestMoneyFrom = console.PromptForInteger("Enter user ID to send to (0 to cancel)");
                if(userIdtoToRequestMoneyFrom == 0)
                {
                    return true;
                }
                if (TheChecker.AreEqual(userIdtoToRequestMoneyFrom, tenmoApiService.UserId))
                {
                    console.PrintError("You can't request money from your self");
                    goto enterID2;
                }
                if(!users.Any((user) => user.UserId == userIdtoToRequestMoneyFrom))
                {
                    console.PrintError("Please select a vaid user id.");
                    goto enterID2;
                }
            enterMoney2:
                decimal amountOfMoneytoBeRequested = console.PromptForDecimal("Enter amount of money");
                if (TheChecker.LeftGreaterthe(0, amountOfMoneytoBeRequested))
                {
                    console.PrintError("You can't send negative money");
                    goto enterMoney2;
                }
                tenmoApiService.TransferPay(tenmoApiService.UserId, userIdtoToRequestMoneyFrom, amountOfMoneytoBeRequested, false);
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();
                console.PrintSuccess("You are now logged out");
            }

            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = tenmoApiService.Login(loginUser);
                if (user == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();
        }

        private Dictionary<int, string> CreateAccountToUsernameLookup(List<Transfer> transfers)
        {
            var userNameLookup = new Dictionary<int, string>();
            userNameLookup[tenmoApiService.UserAccountId] = tenmoApiService.Username;

            foreach (Transfer t in transfers)
            {
                int otherAccountId = (t.To != tenmoApiService.UserAccountId) ? t.To : t.From;

                if (!userNameLookup.ContainsKey(otherAccountId))
                {
                    userNameLookup[otherAccountId] = tenmoApiService.GetUsernameByAccountId(otherAccountId);
                }
            }
            return userNameLookup;
        }
    }
}
