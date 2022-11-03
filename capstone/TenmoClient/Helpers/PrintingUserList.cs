using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Models;

namespace TenmoClient.Helpers
{
   public static class PrintingUserList
    {
        public static void PrintUsers(List<User> users)
        {
            Console.WriteLine("|-------------USERS-----------|");
            Console.WriteLine("|   Id     |     Username     |");
            Console.WriteLine("|----------+------------------|");
            foreach (var user in users)
            {
                Console.WriteLine($"|  {user.UserId,-5}   | {user.Username,-15}  |");
            }

            Console.WriteLine("|-----------------------------|");
        }


    }
}
