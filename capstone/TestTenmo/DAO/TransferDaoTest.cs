using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TestTenmo.DAO
{
    [TestClass]
    public class TransferDaoTest:BaseDaoTests
    {
        private TransferSqlDao dao;

        [TestInitialize]
        public override void Setup()
        {
            dao = new TransferSqlDao(ConnectionString);
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
            Assert.AreEqual(999, transfer.Amount);

        }




    }
}
