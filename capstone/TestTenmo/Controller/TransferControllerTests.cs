using Microsoft.VisualStudio.TestTools.UnitTesting;
using shared;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestTenmo.Controller
{
    [TestClass]
    public class TransferControllerTests : BaseControllerTests
    {
        const string GET_TRANSFERS_BY_USER_URI = "transfer/user/";
        const string POST_TRANSFER_URI = "transfer/SendMoney/";
        const string APPROVE_REJECT_URI = "transfer/AppRej/";

        [TestMethod]
        public async Task GetTransfersByUser_UnauthenticatedUserReturnsUnauthorized()
        {
            var response = await client.GetAsync(GET_TRANSFERS_BY_USER_URI + 1001);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "status code should be 401 Unauthorized if user is not authenticated.");
        }
        [TestMethod]
        public async Task GetTransfersByUser_AuthenticatedUserCanSeeOwnTransfers()
        {
            await Login("mike", "test");
            var response = await client.GetAsync(GET_TRANSFERS_BY_USER_URI + 1001);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "status code should be 200 OK if user is authenticated and requesting thier own transfers.");

        }
        [TestMethod]
        public async Task GetTransfersByUser_AuthenticatedUserCannotSeeOthersTransfers()
        {
            await Login("mike", "test");
            var response = await client.GetAsync(GET_TRANSFERS_BY_USER_URI + 1002);
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode, "status code should be 403 forbbiden if transfers dont belong to user.");
        }
        [TestMethod]
        public async Task PostTransfer_UnauthenticatedUserReturnsUnauthorized()
        {
            TransferRequest request = new TransferRequest(1001, 1002, 100, true);
            var response = await client.PostAsJsonAsync(POST_TRANSFER_URI, request);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "status code should be 401 Unauthorized if user is not authenticated.");
        }
        [TestMethod]
        public async Task PostTransfer_UserCanOnlyMakeTransferFromOwnAccount()
        {
            await Login("mike", "test");

            TransferRequest request = new TransferRequest(1002, 1001, 1, true);
            var response = await client.PostAsJsonAsync(POST_TRANSFER_URI, request);
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode, "User should only be able to send money from own account.");

            request = new TransferRequest(1001, 1002, 1, true);
            response = await client.PostAsJsonAsync(POST_TRANSFER_URI, request);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "User failed to send money from thier account.");
        }
        [TestMethod]
        public async Task PostTransfer_CannotSendMoreMoneyThanTheyHave()
        {
            TransferRequest request = new TransferRequest(1001, 1002, 9999, true);
            await Login("mike", "test");
            var response = await client.PostAsJsonAsync(POST_TRANSFER_URI, request);
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode, "user cannot send more money than they have.");
        }
        [TestMethod]
        public async Task PostTransfer_CannotSendOrRequestNegetiveMoney()
        {
            TransferRequest request = new TransferRequest(1001, 1002, -9999, true);
            await Login("mike", "test");
            var response = await client.PostAsJsonAsync(POST_TRANSFER_URI, request);
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode, "user cannot send  a negetive amount of money.");
        }
        [TestMethod]
        public async Task PostTransfer_CannotSendOrRequestMoneyToSameUser()
        {
            TransferRequest request = new TransferRequest(1001, 1001, 1, true);
            await Login("mike", "test");
            var response = await client.PostAsJsonAsync(POST_TRANSFER_URI, request);
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode, "user cannot send  a negetive amount of money.");
        }
        [TestMethod]
        public async Task ApproveReject_UnauthorizedUserReturnsUnauthorized()
        {
            var transferApproval = new TransferAppRej(3004, 2, 2001);
            var response = await client.PostAsJsonAsync(APPROVE_REJECT_URI, transferApproval);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "status code should be 401 Unauthorized if user is not authenticated.");
        }
        [TestMethod]
        public async Task ApproveReject_OnlyPendingTransferCanBeApprovedRejected()
        {
            await Login("mike", "test");
            var transferApproval = new TransferAppRej(3001, 2, 2001);
            var response = await client.PostAsJsonAsync(APPROVE_REJECT_URI, transferApproval);
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode, "status code should be 403 Forbbiden transfer not pending.");
        }
    }
}
