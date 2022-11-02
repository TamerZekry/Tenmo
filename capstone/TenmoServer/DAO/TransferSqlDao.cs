using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
        private readonly string connectionString;

        public TransferSqlDao(string connString)
        {
            connectionString = connString;
        }
        public List<Transfer> GetPendingTransfers(int userId)
        {
            List<Transfer> transferList = new List<Transfer>();

            using (SqlConnection transferConnection = new SqlConnection(connectionString))
            {
                transferConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM transfer" +
                " JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id" +
                "JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id" +
                " WHERE account_to = 1 AND transfer.transfer_status_id = 1;");
                cmd.Parameters.AddWithValue("@transferId", userId);
                cmd.Connection = transferConnection;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Transfer transfer = null;
                    transfer = CreateTransferFromReader(reader);
                    transferList.Add(transfer);
                }
            }

            return transferList;

        }

        public Transfer GetTransferById(int id)
        {
            Transfer transfer = new Transfer();            

            using (SqlConnection transferConnection = new SqlConnection(connectionString))
            {
                transferConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM transfer" + 
                " JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id" +
                "JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id" +
                " WHERE transfer_id = @transferId;");
                cmd.Parameters.AddWithValue("@transferId", id);
                cmd.Connection = transferConnection;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    transfer = CreateTransferFromReader(reader);
                }
            }

            return transfer;
        }

        public List<Transfer> GetTransfersForUser(int userId)
        {
            throw new NotImplementedException();
        }

        public void RequestTransfer(int requestUserId, int otherUserId)
        {
            throw new NotImplementedException();
        }

        public bool SendTransfer(int userId, int otherUserId, decimal amount)
        {
            throw new NotImplementedException();
        }
        private Transfer CreateTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer();
            transfer.Id = Convert.ToInt32(reader["transfer_id"]);
            transfer.From = Convert.ToString(reader["account_from"]);
            transfer.To = Convert.ToString(reader["account_to"]);
            transfer.Type = Convert.ToString(reader["transfer_type"]);
            transfer.Status = Convert.ToString(reader["transfer_status_desc"]);
            transfer.Amount = Convert.ToDecimal(reader["amount"]);
            return transfer;
        }
    }
}
