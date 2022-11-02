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
                " WHERE account_to = @accountTo AND transfer.transfer_status_id = 1;");
                cmd.Parameters.AddWithValue("@accountTo", userId);
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
            List<Transfer> transferList = new List<Transfer>();

            using (SqlConnection transferConnection = new SqlConnection(connectionString))
            {
                transferConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM transfer" +
                " JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id" +
                "JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id" +
                " WHERE account_to = @userId OR account_from = @userId");
                cmd.Parameters.AddWithValue("@UserId", userId);
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

        public void RequestTransfer(int requestUserId, int otherUserId, decimal amount, bool sending)
        {
            decimal sendersBalance;
            decimal recieversBalance;
            int senderId;
            int recieverId;
            if (sending == true)
            {
                senderId = requestUserId;
                recieverId = otherUserId;
                Account account = null;
                using (SqlConnection accountConnection = new SqlConnection(connectionString))
                {
                    accountConnection.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM account" +
                    " WHERE account_id = @id;");
                    cmd.Parameters.AddWithValue("@id", requestUserId);
                    cmd.Connection = accountConnection;
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        account = CreateAccountFromReader(reader);
                    }

                    sendersBalance = account.balance; //sendersBalance is person sending money

                    SqlCommand cmd1 = new SqlCommand("SELECT * FROM account" +
                    " WHERE account_id = @id;");
                    cmd1.Parameters.AddWithValue("@id", requestUserId);
                    cmd1.Connection = accountConnection;
                    SqlDataReader reader1 = cmd.ExecuteReader();

                    if (reader1.Read())
                    {
                        account = CreateAccountFromReader(reader);
                    }

                    recieversBalance = account.balance; //person recieveing money
                }
            }
            else
            {
                senderId = otherUserId;
                recieverId = requestUserId;
                Account account = null;
                using (SqlConnection accountConnection = new SqlConnection(connectionString))
                {
                    accountConnection.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM account" +
                    " WHERE account_id = @id;");
                    cmd.Parameters.AddWithValue("@id", requestUserId);
                    cmd.Connection = accountConnection;
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        account = CreateAccountFromReader(reader);
                    }

                    recieversBalance = account.balance;

                    SqlCommand cmd1 = new SqlCommand("SELECT * FROM account" +
                    " WHERE account_id = @id;");
                    cmd1.Parameters.AddWithValue("@id", requestUserId);
                    cmd1.Connection = accountConnection;
                    SqlDataReader reader1 = cmd.ExecuteReader();

                    if (reader1.Read())
                    {
                        account = CreateAccountFromReader(reader);
                    }

                    sendersBalance = account.balance;
                }
            }
            sendersBalance -= amount;
            recieversBalance += amount;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd1 = new SqlCommand("UPDATE account SET balance = @balanceRecieve WHERE account_id = @recieversId", conn);
                SqlCommand cmd2= new SqlCommand("UPDATE account SET balance = @balanceSend WHERE account_id = @sendersId", conn);

                cmd1.Parameters.AddWithValue("@balanceRecieve", recieversBalance);
                cmd1.Parameters.AddWithValue("@recieversId", recieverId);
                cmd2.Parameters.AddWithValue("@balanceSend", sendersBalance);
                cmd2.Parameters.AddWithValue("@sendersId", senderId);
                cmd1.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();

            }
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
        private Account CreateAccountFromReader(SqlDataReader reader)
        {
            Account account = new Account();
            account.accoundId = Convert.ToInt32(reader["account_id"]);
            account.userId = Convert.ToInt32(reader["user_id"]);
            account.balance = Convert.ToDecimal(reader["balance"]);
            return account;
        }

        public bool CheckIfAccountExists(int accountId)
         {
            Account account = null;

            using (SqlConnection transferConnection = new SqlConnection(connectionString))
            {
                transferConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM account" +
                " WHERE account_id = @id;");
                cmd.Parameters.AddWithValue("@id", accountId);
                cmd.Connection = transferConnection;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    account = CreateAccountFromReader(reader);
                }
            }
            if(account == null)
            {
                return false;
            }
            return true;
        }
        public Account getAccountById(int id)
        {
            Account account = null;

            using (SqlConnection transferConnection = new SqlConnection(connectionString))
            {
                transferConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM account" +
                " WHERE account_id = @id;");
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Connection = transferConnection;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    account = CreateAccountFromReader(reader);
                }
            }
            return account;
        }
        public bool checkIfCanAfford(int id, Decimal amount)
        {
            Account account = getAccountById(id);
            if (account.balance < amount)
            {
                return false;
            }
            return true;
        }
    }
}
