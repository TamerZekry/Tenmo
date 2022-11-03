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

                SqlCommand cmd = new SqlCommand("SELECT * FROM transfer " +
                "JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id " +
                "JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id " +
                "JOIN account ON transfer.account_to = account.account_id " +
                "JOIN tenmo_user ON tenmo_user.user_id = account.account_id " +
                "WHERE user_id = @accountTo AND transfer.transfer_status_id = 1;");
                cmd.Parameters.AddWithValue("@accountTo", userId);
                cmd.Connection = transferConnection;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Transfer transfer = CreateTransferFromReader(reader);
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

                /*
                SqlCommand cmd = new SqlCommand("SELECT * FROM transfer" +
                " JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id" +
                " JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id" +
                " WHERE account_to = @userId OR account_from = @userId");
                */
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM transfer " +
                    "JOIN account ON account.user_id = @userId " +
                    "JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id " +
                    "JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id " +
                    "WHERE transfer.account_from = account.account_id OR transfer.account_to = account.account_id;");

                cmd.Parameters.AddWithValue("@userId", userId);
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

        public Transfer RequestTransfer(int requestUserId, int otherUserId, decimal amount, bool sending)
        {
            decimal sendersBalance;
            decimal recieversBalance;
            Account sendersAccount = new Account();
            Account recieversAccount = new Account();
            int senderId;
            int recieverId;
            string insertTransfer;
            SqlCommand insertCmd;


            if (sending == true)
            {
                senderId = requestUserId;
                recieverId = otherUserId;                
                using (SqlConnection accountConnection = new SqlConnection(connectionString))
                {
                    accountConnection.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM account" +
                    " JOIN tenmo_user ON account.user_id = tenmo_user.user_id" +
                    " WHERE user_id = @id;");
                    cmd.Parameters.AddWithValue("@id", requestUserId);
                    cmd.Connection = accountConnection;
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        sendersAccount = CreateAccountFromReader(reader);
                    }

                    sendersBalance = sendersAccount.balance; //sendersBalance is person sending money

                    SqlCommand cmd1 = new SqlCommand("SELECT * FROM account" +
                    " JOIN tenmo_user ON account.user_id = tenmo_user.user_id" +
                    " WHERE user_id = @id;");
                    cmd1.Parameters.AddWithValue("@id", otherUserId);
                    cmd1.Connection = accountConnection;
                    SqlDataReader reader1 = cmd.ExecuteReader();

                    if (reader1.Read())
                    {
                        recieversAccount = CreateAccountFromReader(reader);
                    }

                    recieversBalance = recieversAccount.balance; //person recieveing money


                    Transfer transfer = new Transfer();
                    transfer.Amount = amount;
                    transfer.From = sendersAccount.accoundId;
                    transfer.To = recieversAccount.accoundId;
                    transfer.Type = "Send";
                    transfer.Status = "Approved";

                    insertTransfer = "INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) OUTPUT INSERTED.* VALUES (@typeId, @statusId, @accountFrom, @accountTo)";
                                           

                    insertCmd = new SqlCommand(insertTransfer, accountConnection);
                    insertCmd.Parameters.AddWithValue("@typeId", 2);
                    insertCmd.Parameters.AddWithValue("@statusId", 2);
                    insertCmd.Parameters.AddWithValue("@accountFrom", sendersAccount.accoundId);
                    insertCmd.Parameters.AddWithValue("@accointTo", recieversAccount.accoundId);
                }
            }
            else
            {
                senderId = otherUserId;
                recieverId = requestUserId;
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
                        recieversAccount = CreateAccountFromReader(reader);
                    }

                    recieversBalance = recieversAccount.balance;

                    SqlCommand cmd1 = new SqlCommand("SELECT * FROM account" +
                    " WHERE account_id = @id;");
                    cmd1.Parameters.AddWithValue("@id", otherUserId);
                    cmd1.Connection = accountConnection;
                    SqlDataReader reader1 = cmd.ExecuteReader();

                    if (reader1.Read())
                    {
                        sendersAccount = CreateAccountFromReader(reader);
                    }

                    sendersBalance = sendersAccount.balance;
                    Transfer transfer = new Transfer();
                    transfer.Amount = amount;
                    transfer.From = senderId;
                    transfer.To = recieverId;
                    transfer.Type = "Request";
                    transfer.Status = "Pending";

                    insertTransfer = "INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) OUTPUT INSERTED.* VALUES (@typeId, @statusId, @accountFrom, @accountTo)";


                    insertCmd = new SqlCommand(insertTransfer, accountConnection);
                    insertCmd.Parameters.AddWithValue("@typeId", 1);
                    insertCmd.Parameters.AddWithValue("@statusId", 1);
                    insertCmd.Parameters.AddWithValue("@accountFrom", sendersAccount.accoundId);
                    insertCmd.Parameters.AddWithValue("@accointTo", recieversAccount.accoundId);
                }
            }
            sendersBalance -= amount;
            recieversBalance += amount;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd1 = new SqlCommand
                    ("BEGIN TRANSACTION;" +
                    "UPDATE account SET balance = @balanceRecieve WHERE account_id = @recieversId;" +
                    "UPDATE account SET balance = @balanceSend WHERE account_id = @sendersId;");
             
                SqlCommand commit = new SqlCommand("COMMIT;", conn);

                cmd1.Parameters.AddWithValue("@balanceRecieve", recieversBalance);
                cmd1.Parameters.AddWithValue("@recieversId", recieverId);
                cmd1.Parameters.AddWithValue("@balanceSend", sendersBalance);
                cmd1.Parameters.AddWithValue("@sendersId", senderId);
                cmd1.ExecuteNonQuery();
                SqlDataReader reader = insertCmd.ExecuteReader();
                commit.ExecuteNonQuery();
                Transfer transfer = null;
                if (reader.Read())
                {
                    transfer = CreateTransferFromReader(reader);
                }
                return transfer;
                
            }
        }
      
        private Transfer CreateTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer();
            transfer.Id = Convert.ToInt32(reader["transfer_id"]);
            transfer.From = Convert.ToInt32(reader["account_from"]);
            transfer.To = Convert.ToInt32(reader["account_to"]);
            transfer.Type = Convert.ToString(reader["transfer_type_desc"]);
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
