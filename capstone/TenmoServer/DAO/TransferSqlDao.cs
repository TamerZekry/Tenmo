using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {

        private readonly string connectionString;
        private readonly IUserDao _userDao;

        public TransferSqlDao(string connString)
        {
            connectionString = connString;
            _userDao = new UserSqlDao(connectionString);
        }
        public List<Transfer> GetTransfersForUser(int userId)
        {
            List<Transfer> transferList = new List<Transfer>();

            using (SqlConnection transferConnection = new SqlConnection(connectionString))
            {
                transferConnection.Open();
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
        public Transfer GetTransferById(int id)
        {
            Transfer transfer = new Transfer();
            using (SqlConnection transferConnection = new SqlConnection(connectionString))
            {
                transferConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM transfer  WHERE transfer_id = @transferId");
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
        public List<Transfer> GetPendingTransfers(int userId)
        {
            List<Transfer> transferList = new List<Transfer>();

            using (SqlConnection transferConnection = new SqlConnection(connectionString))
            {
                transferConnection.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT transfer_id, account_from, account_to, transfer_type_desc, transfer_status_desc, amount FROM transfer " +
                    "JOIN account ON account.user_id = @accountTo " +
                    "JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id " +
                    "JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id " +
                    "WHERE transfer.account_to = account.account_id AND transfer.transfer_status_id = 1;");

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

        /// <summary>
        ///  Sending or requesting money Transfer
        /// </summary>
        /// <param name="fromId"> Sending account number   </param>
        /// <param name="toId"> Reciveing account number   </param>
        /// <param name="amount"> Amount of money</param>
        /// <param name="isThisSend"> True for sending or false for receiving </param>
        /// <returns></returns>
        public Transfer SendTransfer(int fromId, int toId, decimal amount, bool isThisSend)
        {
            Transfer result = new Transfer();
            if (isThisSend) //this is send money request
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string commandString = @"
                    BEGIN TRANSACTION; 
                    UPDATE account SET balance -= @amount WHERE account_id = @Sender_ID; 
                    UPDATE account SET balance += @amount WHERE account_id = @Reciever_ID; 
                    INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) 
                    OUTPUT INSERTED.* 
                    VALUES (2,2,@Sender_ID,@Reciever_ID,@Amount);
                    COMMIT;";
                    SqlCommand sqlCommand = new SqlCommand(commandString, conn);
                    sqlCommand.Parameters.AddWithValue("@Amount", amount);
                    sqlCommand.Parameters.AddWithValue("@Sender_ID", _userDao.GetAccountId(fromId));
                    sqlCommand.Parameters.AddWithValue("@Reciever_ID", _userDao.GetAccountId(toId));

                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        result = CreateTransferFromReader(reader);
                    }
                }
            }
            else // this is request money request  AND IT SHOULDN'T CHANGE SENDER OR RECIEVER BALANCE UNTIL its APPROVED   
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string commandString = @"
                    BEGIN TRANSACTION; 
                    INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) 
                    OUTPUT INSERTED.* 
                    VALUES (1,1,@Sender_ID,@Reciever_ID,@Amount);
                    COMMIT;";
                    SqlCommand sqlCommand = new SqlCommand(commandString, conn);
                    sqlCommand.Parameters.AddWithValue("@Amount", amount);
                    sqlCommand.Parameters.AddWithValue("@Sender_ID", _userDao.GetAccountId(toId));
                    sqlCommand.Parameters.AddWithValue("@Reciever_ID", _userDao.GetAccountId(fromId));

                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        result = CreateTransferFromReader(reader);
                    }
                }
            }
            return result;

        }
        public bool ChangeTransferStatus(int transfer_id, int appRej)
        {
            /*
               1: Approve      Pending  > Approved       Change Balance         Change status       
               2: Reject       Pending  > Reject                                Change status
           */
            if (appRej == 1)
            {
                var TransferToApproved = GetTransferById(transfer_id);
                if (TransferToApproved.Amount < _userDao.GetBalanceByAccount(TransferToApproved.From))
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string sqlString =
                            @"BEGIN TRANSACTION; " +
                             "UPDATE account SET balance -= @amount WHERE account_id =  @Sender_ID ; " +
                             "UPDATE account SET balance += @amount WHERE account_id = @Reciever_ID ;" +
                             "UPDATE transfer set transfer_status_id = @statusID WHERE transfer_id = @transID; " +
                             "COMMIT;";
                            SqlCommand sqlCommand = new SqlCommand(sqlString, conn);
                            sqlCommand.Parameters.AddWithValue("@amount", TransferToApproved.Amount);
                            sqlCommand.Parameters.AddWithValue("@Sender_ID", TransferToApproved.From);
                            sqlCommand.Parameters.AddWithValue("@Reciever_ID", TransferToApproved.To);
                            sqlCommand.Parameters.AddWithValue("@statusID", appRej + 1);
                            sqlCommand.Parameters.AddWithValue("@transID", transfer_id);
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return false;
                }
            }

            if (appRej == 2)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(@"UPDATE transfer set transfer_status_id = @statusID WHERE transfer_id = @transID;", conn);
                        cmd.Parameters.AddWithValue("@statusID", appRej + 1);
                        cmd.Parameters.AddWithValue("@transID", transfer_id);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return true;
        }

        private Transfer CreateTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer();
            transfer.Id = Convert.ToInt32(reader["transfer_id"]);
            transfer.From = Convert.ToInt32(reader["account_from"]);
            transfer.To = Convert.ToInt32(reader["account_to"]);
            transfer.Type = Convert.ToInt32(reader["transfer_type_id"]) == 1 ? "Request" : "Send";
            transfer.Status = getStatusStringFromInt(Convert.ToInt32(reader["transfer_status_id"]));
            transfer.Amount = Convert.ToDecimal(reader["amount"]);
            return transfer;
        }
        private string getStatusStringFromInt(int status)
        {
            string result = "";
            result = status switch
            {
                1 => "Pending",
                2 => "Approved",
                _ => "Rejected",
            };
            return result;
        }
    }
}
