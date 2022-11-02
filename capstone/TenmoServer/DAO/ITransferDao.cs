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
        public bool SendTransfer(int userId, int otherUserId, decimal amount);
        public void RequestTransfer(int requestUserId, int otherUserId);
    }
}
