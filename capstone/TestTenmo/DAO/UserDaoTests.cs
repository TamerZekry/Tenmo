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
            Assert.IsNotNull(users,"Expected List of users Not Null");
            Assert.AreEqual(4, users.Count);
        }
        
        [TestMethod]
        public void GetUserByUserName()
        {
            var user = dao.GetUser("tamer");
            Assert.AreEqual(user.UserId,1002);
           
        }
       
        [TestMethod]
        public void AddUser()
        {
            var user = dao.AddUser("TestUser", "test");
            Assert.AreEqual(5, dao.GetUsers().Count);
            Assert.AreEqual(dao.GetUser("TestUser").UserId, 1005);
        }
        [TestMethod]
        public void GetUsernameByAccount()
        {
            var userName = dao.GetUsernameByAcount(2001);
            Assert.AreEqual("mike", userName);
        }

        [TestMethod]
        public void GetBalanceById()
        {
            var userBalance = dao.GetUserBalanceById(1001);
            Assert.AreEqual(1000, userBalance);
        }




    }
}
