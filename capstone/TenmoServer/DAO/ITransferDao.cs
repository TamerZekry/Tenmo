using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        public List<Transfer> GetTransfersForUser(int userId);
        public Transfer GetTransferById(int id);
        public List<Transfer> GetPendingTransfers(int userId);
        public void RequestTransfer(int requestUserId, int otherUserId, decimal amount, bool sending);
        public bool CheckIfAccountExists(int accountId);
        public Account getAccountById(int id);
        public bool checkIfCanAfford(int id, Decimal amount);
    }
}
