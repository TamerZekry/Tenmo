using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoConsoleService : ConsoleService
    {
        /************************************************************
            Print methods
        ************************************************************/
        public void PrintLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }

        public void PrintMainMenu(string username)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"Hello, {username}!");
            Console.WriteLine("1: View your current balance");
            Console.WriteLine("2: View your past transfers");
            Console.WriteLine("3: View your pending requests");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Log out");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }

        public void PrintViewTransfersMenu(List<Transfer> transfers, int accountId)
        {
            string toUsername = string.Empty;
            string fromUsername = string.Empty;
            string otherUsername = string.Empty;

            Console.WriteLine("Transfers");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Id          From/To                 Amount");
            Console.WriteLine("-------------------------------------------");
            foreach (var transfer in transfers)
            {
                string type = "";
                if(transfer.To == accountId)
                {
                    otherUsername = fromUsername;
                    type = "From";
                }
                else
                {
                    otherUsername = toUsername;
                    type = "To";
                }
                Console.WriteLine($"{transfer.Id} {type}: { otherUsername}");
            }
            Console.WriteLine("--------");
        }

        public LoginUser PromptForLogin()
        {
            string username = PromptForString("User name");
            if (String.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            string password = PromptForHiddenString("Password");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        internal void PrintTransferDetails(Transfer transfer)
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Transfer");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Id: {transfer.Id}");
            Console.WriteLine($"From: {transfer.From}");//wrong should be username.
            Console.WriteLine($"To: {transfer.From}");//wrong should be username.
            Console.WriteLine($"Type: {transfer.Type}");
            Console.WriteLine($"Status: {transfer.Status}");
            Console.WriteLine($"Amount: {transfer.Amount}");
        }

        // Add application-specific UI methods here...


    }
}
