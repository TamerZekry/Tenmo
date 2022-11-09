using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenmoClient.Models;
using TenmoServer.DAO;

namespace TestTenmo.DAO
{
    [TestClass]
    public class UserDaoTests : BaseDaoTests
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
            Assert.IsNotNull(users, "Expected List of users Not Null");
            Assert.AreEqual(4, users.Count);
        }
        [TestMethod]
        public void GetUserNameByAccount()
        {
            var user = dao.GetUsernameByAcount(2001);
            Assert.IsNotNull(user, "Expected a username Not Null");
            Assert.AreEqual("mike", user);
        }
        [TestMethod]
        public void GetBalanceByAccount()
        {
            var Balance = dao.GetBalanceByAccount(2001);
            Assert.IsNotNull(Balance, "Expected a balance Not 0");
            Assert.AreEqual(1000, Balance);
        }
        // GetAccountId
        [TestMethod]
        public void GetAccount()
        {
            var AccountId = dao.GetAccountId(1001);
            Assert.IsNotNull(AccountId, "Expected AccountId Not 0");
            Assert.AreEqual(2001, AccountId);
        }
        [TestMethod]
        public void GetUserBalanceByUserId()
        {
            var Balance = dao.GetUserBalanceById(1001);
            Assert.IsNotNull(Balance, "Expected AccountId Not 0");
            Assert.AreEqual(1000, Balance);
        }
        [TestMethod]
        public void GetUserByUserName()
        {
            TenmoServer.Models.User user = dao.GetUser("alex");
            Assert.AreEqual("alex", user.Username);
            Assert.AreEqual("dT+xNXq2MOg=", user.Salt);
            Assert.AreEqual("pKiKVUS6XIQlgSv/WYVNC7tt6vo=", user.PasswordHash);
            Assert.AreEqual(1003, user.UserId);

        }

    }
}
