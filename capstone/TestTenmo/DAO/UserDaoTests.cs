using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoServer.DAO;

namespace TestTenmo.DAO
{
    [TestClass]
    public class UserDaoTests:BaseDaoTests
    {
        private UserSqlDao dao;

        [TestInitialize]
        public override void Setup()
        {
            dao = new UserSqlDao(ConnectionString);
            base.Setup();
        }

        [TestMethod]
        public void GetListOfUsers()  
        {
            var users = dao.GetUsers();
            Assert.IsNull(users,"Expected List of users Not Null");
            Assert.AreEqual(4, users.Count);
        }
    }
}
