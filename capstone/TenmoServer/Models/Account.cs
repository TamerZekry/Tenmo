using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Account
    {
        public int accoundId { get; set; }
        public int userId { get; set; }
        public decimal balance { get; set; }
    }
}
