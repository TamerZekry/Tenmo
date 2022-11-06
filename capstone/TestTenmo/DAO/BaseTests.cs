using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;

namespace TestTenmo.DAO
{
    [TestClass]
    public class BaseDaoTests
    {
        private const string DatabaseName = "tenmoTesting";
        private const string AdminConnectionString = @"Server=.\SQLEXPRESS;Database=master;Trusted_Connection=True;";
        protected const string ConnectionString = @"Server=.\SQLEXPRESS;Database=" + DatabaseName + ";Trusted_Connection=True;";
        private TransactionScope transaction;

        [AssemblyInitialize]  
        public static void BeforeAllTests(TestContext context)
        {
            string sql = File.ReadAllText("create_test_db.sql").Replace("test_db_name", DatabaseName);
            using (SqlConnection conn = new SqlConnection(AdminConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            
            sql = File.ReadAllText("test_mock_data.sql");
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                 cmd.ExecuteNonQuery();
            }
        }

        [AssemblyCleanup]  
        public static void AfterAllTests()
        {
            string sql = File.ReadAllText("drop_test_db.sql").Replace("test_db_name", DatabaseName);
            using (SqlConnection conn = new SqlConnection(AdminConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
        [TestInitialize]  
        public virtual void Setup()
        {
          transaction = new TransactionScope();
        }

        [TestCleanup]  
        public void Cleanup()
        {
            transaction.Dispose();
        }
    }
}
