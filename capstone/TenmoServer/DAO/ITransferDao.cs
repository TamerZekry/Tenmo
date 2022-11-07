using shared.Models;
using System.Collections.Generic;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        public List<Transfer> GetTransfersForUser(int userId);
        public Transfer GetTransferById(int id);
        //public List<Transfer> GetPendingTransfers(int userId);
        public Transfer SendTransfer(int fromId, int toId, decimal amount, bool isThisSend);
        bool ChangeTransferStatus(int transfer, int appRej);
    }
}
