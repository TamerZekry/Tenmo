using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using shared.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TestTenmo.DAO
{
    [TestClass]
    public class TransferDaoTest:BaseDaoTests
    {
        private TransferSqlDao dao;
        private UserSqlDao dao_user;

        [TestInitialize]
        public override void Setup()
        {
            dao = new TransferSqlDao(ConnectionString);
            dao_user = new UserSqlDao(ConnectionString);
            base.Setup();
        }
        [TestMethod]
        public void GetTransferById()
        {
            Transfer transfer = dao.GetTransferById(3001);
            Assert.IsNotNull(transfer);
            Assert.AreEqual(3001, transfer.Id);
            Assert.AreEqual("Send", transfer.Type);
            Assert.AreEqual("Approved", transfer.Status);
            Assert.AreEqual(2002, transfer.From);
            Assert.AreEqual(2001, transfer.To);
            Assert.AreEqual(50, transfer.Amount);

        }

        [TestMethod]
        [DataRow(1, "Pending")]
        [DataRow(2, "Approved")]
        [DataRow(3, "Rejected")]
        [DataRow(4, "Rejected")]
        public void GetStatusStringFromInt(int statusNum, string statusString)
        {
            string reuslt = dao.GetStatusStringFromInt(statusNum);
            Assert.AreEqual(statusString, reuslt);
        }

        [TestMethod]
        public void ListAllTransfer()
        {
            List<Transfer> transferList = dao.GetTransfersForUser(1002);
            Assert.AreEqual(3, transferList.Count);
        }

        [TestMethod]
        public void SendTranserAsSend()
        {
            Transfer transfer = null;
                using (SqlConnection conn = new SqlConnection(BaseDaoTests.ConnectionString))
                {
                    conn.Open();
                    string commandString = @"
                    UPDATE account SET balance -= @amount WHERE account_id = @Sender_ID; 
                    UPDATE account SET balance += @amount WHERE account_id = @Reciever_ID; 
                    INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) 
                    OUTPUT INSERTED.* 
                    VALUES (2,2,@Sender_ID,@Reciever_ID,@Amount);";
                    
                    SqlCommand sqlCommand = new SqlCommand(commandString, conn);
                    sqlCommand.Parameters.AddWithValue("@Amount", 50);
                    sqlCommand.Parameters.AddWithValue("@Sender_ID", 2001);
                    sqlCommand.Parameters.AddWithValue("@Reciever_ID", 2002);

                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.Read())
                    {
                    transfer = dao.CreateTransferFromReader(reader);
                    }
                }

            Assert.IsNotNull(transfer);
            Assert.AreEqual(3005, transfer.Id);
            Assert.AreEqual("Send", transfer.Type);
            Assert.AreEqual("Approved", transfer.Status);
            Assert.AreEqual(2001, transfer.From);
            Assert.AreEqual(2002, transfer.To);
            Assert.AreEqual(50, transfer.Amount);

        }
        [TestMethod]
        public void SendTranserAsRequest()
        {
            Transfer transfer = null;
            using (SqlConnection conn = new SqlConnection(BaseDaoTests.ConnectionString))
            {
                conn.Open();
                string commandString = @"
                    UPDATE account SET balance -= @amount WHERE account_id = @Sender_ID; 
                    UPDATE account SET balance += @amount WHERE account_id = @Reciever_ID; 
                    INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) 
                    OUTPUT INSERTED.* 
                    VALUES (1,1,@Sender_ID,@Reciever_ID,@Amount);";

                SqlCommand sqlCommand = new SqlCommand(commandString, conn);
                sqlCommand.Parameters.AddWithValue("@Amount", 50);
                sqlCommand.Parameters.AddWithValue("@Sender_ID", 2001);
                sqlCommand.Parameters.AddWithValue("@Reciever_ID", 2002);

                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    transfer = dao.CreateTransferFromReader(reader);
                }
            }

            Assert.IsNotNull(transfer);
            Assert.AreEqual(3005, transfer.Id);
            Assert.AreEqual("Request", transfer.Type);
            Assert.AreEqual("Pending", transfer.Status);
            Assert.AreEqual(2001, transfer.From);
            Assert.AreEqual(2002, transfer.To);
            Assert.AreEqual(50, transfer.Amount);

        }

        [TestMethod]
        public void CheckBalanceBeforeAndAfterSendingMoney()
        {
            decimal balanceBeforeTransfer = dao_user.GetBalanceByAccount(2002);
            using (SqlConnection conn = new SqlConnection(BaseDaoTests.ConnectionString))
            {
                conn.Open();
                string commandString = @"
                    UPDATE account SET balance -= @amount WHERE account_id = @Sender_ID; 
                    UPDATE account SET balance += @amount WHERE account_id = @Reciever_ID; 
                    INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) 
                    OUTPUT INSERTED.* 
                    VALUES (2,2,@Sender_ID,@Reciever_ID,@Amount);";
                SqlCommand sqlCommand = new SqlCommand(commandString, conn);
                sqlCommand.Parameters.AddWithValue("@Amount", 50);
                sqlCommand.Parameters.AddWithValue("@Sender_ID", 2001);
                sqlCommand.Parameters.AddWithValue("@Reciever_ID", 2002);
                SqlDataReader reader = sqlCommand.ExecuteReader();
            }
            decimal balanceAfterTransfer = dao_user.GetBalanceByAccount(2002);
            Assert.AreEqual(50, balanceAfterTransfer - balanceBeforeTransfer);

        }


    }
}
