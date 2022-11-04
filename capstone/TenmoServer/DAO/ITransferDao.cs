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
        //public Transfer RequestTransfer(int requestUserId, int otherUserId, decimal amount, bool sending);
        public bool CheckIfAccountExists(int accountId);
        public Account GetAccountById(int id);
        public bool CheckIfCanAfford(int id, Decimal amount);
        public Transfer SendTransfer(int fromId, int toId, decimal amount);
        public Transfer RequestTransfer(int fromId, int toId, decimal amount);
        public Transfer CreateTransferFromValues(string type, decimal amount, Account sender, Account reciever);
        public Account CreateAccountFromId(int id);
    }
}
