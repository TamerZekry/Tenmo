﻿using System;
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
                console.Pause("Your current account balance is :" + tenmoApiService.getBalanceById(tenmoApiService.UserId).ToString("C"));
            }

            if (menuSelection == 2)
            {
                var transferList = tenmoApiService.GetAllTransfersForUser();
                // View your past transfers
                console.PrintViewTransfersMenu(
                    transfers: transferList,
                    accountId: tenmoApiService.GetAccountById(tenmoApiService.UserId));

                Transfer selectedTransfer = null;

                do
                {
                    int selectedId = console.PromptForInteger("Please enter transfer ID to view details (0 to cancel): ");
                    if (selectedId == 0)
                    {
                        break;
                    }

                    selectedTransfer = transferList.Where((t => t.Id == selectedId)).FirstOrDefault();
                    if (selectedTransfer == null)
                    {
                        console.PrintError("Please select a valid transfer Id");
                    }
                }
                while (selectedTransfer == null);

                //View Transfer details.
                console.PrintTransferDetails(selectedTransfer);
            }

            if (menuSelection == 3)
            {
                // View your pending requests
            }

            if (menuSelection == 4)
            {
                PrintingUserList.PrintUsers( tenmoApiService.GetAllUsers());
                enterID:
                int userIdtoSendMonyTo =   console.PromptForInteger("Enter user ID to send to");
                if (TheChecker.AreEqual( userIdtoSendMonyTo , tenmoApiService.UserId))
                {
                    console.PrintError("You can't send money to your self");
                    goto enterID;
                }
                enterMoney:
                decimal AmountOfMoneytoBeSend = console.PromptForDecimal("Enter amount of money");
                if (TheChecker.LeftGreaterthe(0, AmountOfMoneytoBeSend))
                {
                    console.PrintError("You can't send negative money");
                    goto enterMoney;
                }
                //call   transfer



                tenmoApiService.TransferPay(tenmoApiService.UserId, userIdtoSendMonyTo, AmountOfMoneytoBeSend);


                console.Pause();
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
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
    }




}
