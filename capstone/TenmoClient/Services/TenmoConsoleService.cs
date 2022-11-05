using System;
using System.Collections.Generic;
using System.Linq;
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

        public void PrintViewTransfersMenu(List<Transfer> transfers, int accountId, Dictionary<int, string> userNameLookup)
        {
            Console.WriteLine("Transfers");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Id          From/To                 Amount");
            Console.WriteLine("-------------------------------------------");

            string otherUsername;
            string type;

            foreach (var transfer in transfers)
            {
                if (transfer.To == accountId)
                {
                    otherUsername = userNameLookup[transfer.From];
                    type = "From";
                }
                else
                {
                    otherUsername = userNameLookup[transfer.To];
                    type = "  To";
                }
                Console.WriteLine($"{transfer.Id,-12} {type}: {otherUsername,-15} {transfer.Amount:C}");
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

        public Transfer PromptForTransfer(List<Transfer> transfersList, string message)
        {
            Transfer selectedTransfer = null;

            do
            {
                int selectedId = PromptForInteger(message);
                if (selectedId == 0)
                {
                    break;
                }

                selectedTransfer = transfersList.FirstOrDefault((t => t.Id == selectedId));
                if (selectedTransfer == null)
                {
                    PrintError("Please select a valid transfer Id");
                }
            }
            while (selectedTransfer == null);

            return selectedTransfer;
        }

        public void PrintTransferDetails(Transfer transfer, Dictionary<int, string> usernameLookup)
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Transfer");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Id: {transfer.Id}");
            Console.WriteLine($"From: {usernameLookup[transfer.From]}");//wrong should be username.
            Console.WriteLine($"To: {usernameLookup[transfer.To]}");//wrong should be username.
            Console.WriteLine($"Type: {transfer.Type}");
            Console.WriteLine($"Status: {transfer.Status}");
            Console.WriteLine($"Amount: {transfer.Amount}");
            Pause();
        }

        public void PrintPendingTransferMenu(List<Transfer> pendingTransfers, Dictionary<int, string> userNameLookup)
        {
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Pending Transfers");
            Console.WriteLine("ID          To                     Amount");
            Console.WriteLine("-------------------------------------------");

            foreach (var transfer in pendingTransfers)
            {
                Console.WriteLine($"{transfer.Id,-8}{userNameLookup[transfer.To],-16}{transfer.Amount:C}");
            }

            Console.WriteLine("---------");
        }

        public void PrintApproveOrReject()
        {
            Console.WriteLine("1: Approve");
            Console.WriteLine("2: Reject");
            Console.WriteLine("0: Don't approve or reject");
            Console.WriteLine("---------");
        }
    }
}
