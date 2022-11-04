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
        public List<Transfer> GetPendingTransfers(int userId)
        {
            List<Transfer> transferList = new List<Transfer>();

            using (SqlConnection transferConnection = new SqlConnection(connectionString))
            {
                transferConnection.Open();
                /*
                SqlCommand cmd = new SqlCommand("SELECT * FROM transfer " +
                "JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id " +
                "JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id " +
                "JOIN account ON transfer.account_to = account.account_id " +
                "JOIN tenmo_user ON tenmo_user.user_id = account.account_id " +
                "WHERE user_id = @accountTo AND transfer.transfer_status_id = 1;");
                */

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

                    SqlCommand cmd = new SqlCommand("SELECT account.account_id, account.user_id, account.balance FROM account" +
                  " JOIN tenmo_user ON account.user_id = tenmo_user.user_id" +
                  " WHERE tenmo_user.user_id = @id;");

                    cmd.Parameters.AddWithValue("@id", requestUserId);
                    cmd.Connection = accountConnection;
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        sendersAccount = CreateAccountFromReader(reader);
                    }

                    sendersBalance = sendersAccount.balance; //sendersBalance is person sending money

                    SqlCommand cmd1 = new SqlCommand("SELECT  account.account_id,account.user_id,account.balance FROM account" +
                    " JOIN tenmo_user ON account.user_id = tenmo_user.user_id" +
                    " WHERE tenmo_user.user_id = @id;");
                    cmd1.Parameters.AddWithValue("@id", otherUserId);
                    cmd1.Connection = accountConnection;
                    SqlDataReader reader1 = cmd1.ExecuteReader();

                    if (reader1.Read())
                    {
                        recieversAccount = CreateAccountFromReader(reader1);
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
<<<<<<< HEAD
                    "UPDATE account SET balance = @balanceSend WHERE account_id = @sendersId;", conn);
             
                SqlCommand commit = new SqlCommand("COMMIT;", conn);
=======
                    "UPDATE account SET balance = @balanceSend WHERE account_id = @sendersId; COMMIT;", conn);

                //   SqlCommand commit = new SqlCommand("COMMIT;", conn);
>>>>>>> 7ae2a80d37bff2b6e9b986ca804e3648f1508539

                cmd1.Parameters.AddWithValue("@balanceRecieve", recieversBalance);
                cmd1.Parameters.AddWithValue("@recieversId", recieverId);
                cmd1.Parameters.AddWithValue("@balanceSend", sendersBalance);
                cmd1.Parameters.AddWithValue("@sendersId", senderId);
                cmd1.ExecuteNonQuery();
                SqlDataReader reader = insertCmd.ExecuteReader();
                //   commit.ExecuteNonQuery();
                Transfer transfer = null;
                if (reader.Read())
                {
                    transfer = CreateTransferFromReader(reader);
                }
                return transfer;
            }
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
            if (account == null)
            {
                return false;
            }
            return true;
        }
        public Account GetAccountById(int id)
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
        public bool CheckIfCanAfford(int id, Decimal amount)
        {
            Account account = GetAccountById(id);
            if (account.balance < amount)
            {
                return false;
            }
            return true;
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
                /* UPDATE account SET balance -= @amount WHERE account_id =  @Sender_ID ; 
                   UPDATE account SET balance += @amount WHERE account_id = @Reciever_ID  ; */
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
                    sqlCommand.Parameters.AddWithValue("@Sender_ID", _userDao.GetAccountId( toId ));
                    sqlCommand.Parameters.AddWithValue("@Reciever_ID", _userDao.GetAccountId(fromId));

                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        result = CreateTransferFromReader(reader);
                    }
                }
            }

            return result;
            #region OLD CODE TO 
            //int sender = _userDao.GetAccountId(fromId);
            //int reciever = _userDao.GetAccountId(toId);
            //Transfer transfer = CreateTransferFromValues("Send", amount, sender, reciever);
            //using (SqlConnection accountConnection = new SqlConnection(connectionString))
            //{
            //    accountConnection.Open();
            //  //  SqlCommand cmd = CreateTransferSql(sender, reciever, transfer);
            //  //  cmd.Connection = accountConnection;
            //  //  SqlDataReader reader = cmd.ExecuteReader();
            //    //if (reader.Read())
            //    //{
            //    //    transfer = CreateTransferFromReader(reader);
            //    //}

            //}
            //return null; // transfer; 
            #endregion
        }
        public void ChangeTransferStatus(int transfer, int appRej)
        {
            /*
               1: Approve
               2: Reject
            */
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"UPDATE transfer set transfer_status_id = @statusID WHERE transfer_id = @transID;", conn);
                cmd.Parameters.AddWithValue("@statusID", appRej + 1);
                cmd.Parameters.AddWithValue("@transID",transfer);
                cmd.ExecuteNonQuery();
            }
        }



        public Transfer RequestTransfer(int fromId, int toId, decimal amount)
        {
            Account sender = CreateAccountFromId(fromId);
            Account reciever = CreateAccountFromId(toId);
            Transfer transfer = CreateTransferFromValues("Request", amount, sender, reciever);

            using (SqlConnection accountConnection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = CreateTransferSql(sender, reciever, transfer);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    transfer = CreateTransferFromReader(reader);
                }
                return transfer;
            }
        }
        public Transfer CreateTransferFromValues(string type, decimal amount, Account sender, Account reciever)
        {
            Transfer transfer = new Transfer();
            transfer.Amount = amount;
            transfer.From = sender.accoundId;
            transfer.To = reciever.accoundId;
            if (type.Equals("Send"))
            {
                transfer.Type = "Send";
                transfer.Status = "Approved";

            }
            else
            {
                transfer.Type = "Request";
                transfer.Status = "Pending";
            }
            return transfer;
        }
        public Account CreateAccountFromId(int id)
        {
            Account account = new Account();

            using (SqlConnection accountConnection = new SqlConnection(connectionString))
            {
                accountConnection.Open();
                SqlCommand cmd = new SqlCommand(
                "SELECT account.account_id, account.user_id, account.balance FROM account " +
                "JOIN tenmo_user ON account.user_id = tenmo_user.user_id " +
                "WHERE account.user_id = @id;", accountConnection);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    account = CreateAccountFromReader(reader);
                }
            }
            return account;
        }
        private SqlCommand CreateTransferSql(Account sender, Account reciever, Transfer transfer)
        {
            int transferType = 1;
            string sqlString = "";
            if (transfer.Type == "Send")
            {
                transferType = 2;
                sqlString = "UPDATE account SET balance = @balanceRecieve WHERE account_id = @recieversId; " +
                            "UPDATE account SET balance = @balanceSend WHERE account_id = @sendersId; ";
            }
            sqlString += "INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) OUTPUT INSERTED.* VALUES (@typeId, @statusId, @accountFrom, @accountTo, @amount) ";
            SqlCommand cmd = new SqlCommand
                (
                "BEGIN TRANSACTION; " +
                sqlString +
                "COMMIT;"
                );
            cmd.Parameters.AddWithValue("@balanceRecieve", reciever.balance + transfer.Amount);
            cmd.Parameters.AddWithValue("@recieversId", reciever.accoundId);
            cmd.Parameters.AddWithValue("@balanceSend", sender.balance - transfer.Amount);
            cmd.Parameters.AddWithValue("@sendersId", sender.accoundId);
            cmd.Parameters.AddWithValue("@typeId", transfer.Type);
            cmd.Parameters.AddWithValue("@statusId", transferType);
            cmd.Parameters.AddWithValue("@accountFrom", sender.accoundId);
            cmd.Parameters.AddWithValue("@accountTo", reciever.accoundId);
            cmd.Parameters.AddWithValue("@amount", transfer.Amount);

            return cmd;
        }
<<<<<<< HEAD
       
/*
=======

>>>>>>> 7ae2a80d37bff2b6e9b986ca804e3648f1508539
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
*/
    }
}
