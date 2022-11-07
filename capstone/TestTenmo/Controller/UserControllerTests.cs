using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TenmoClient.Models;

namespace TestTenmo.Controller
{
    [TestClass]
    public class UserControllerTests : BaseControllerTests
    {
        const string GET_USERNAME_FROM_ACCOUNT_ID_URL = "user/username/account/";
        [TestMethod]
        public async Task Get_ReturnsUnauthorizedIfNotAuthenticated()
        {
            var response = await client.GetAsync("user");
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "status code should be 401 Unauthorized if user is not authenticated.");
        }

        [TestMethod]
        public async Task GetUserNameByAccount_ReturnsUnauthorizedIfNotAuthenticated()
        {
            var response = await client.GetAsync(GET_USERNAME_FROM_ACCOUNT_ID_URL + "2001");
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "status code should be 401 Unauthorized if user is not authenticated.");
        }
        [TestMethod]
        public async Task GetUserNameByAccount_ReturnsNoContentIfUserDoesntExist()
        {
            ApiUser user = await Login("mike", "test");
            var response = await client.GetAsync(GET_USERNAME_FROM_ACCOUNT_ID_URL + "0000");
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode, "status code should be 204 NoContent if account does not exist and user is authenticated.");
        }
        [TestMethod]
        public async Task GetUserNameByAccount_ReturnsOkIfUserExist()
        {
            ApiUser user = await Login("mike", "test");
            var response = await client.GetAsync(GET_USERNAME_FROM_ACCOUNT_ID_URL + "2001");
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "status code should be 200 OK if account does exists and user is authenticated.");
        }
    }
}
