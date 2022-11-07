using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using TestTenmo.DAO;
using TenmoClient.Models;
using Newtonsoft.Json;

namespace TestTenmo.Controller
{
    public class BaseControllerTests : BaseDaoTests
    {
        protected HttpClient client;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            var builder = new WebHostBuilder()
                .UseStartup<TenmoServer.Startup>()
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.test.json")
                    .Build());

            var server = new TestServer(builder);
            client = server.CreateClient();
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
            Logout();
        }

        protected async Task<ApiUser> Login(string username, string password)
        {
            TenmoServer.Models.LoginUser user = new TenmoServer.Models.LoginUser() { Username = username, Password = password };
            var response = await client.PostAsJsonAsync("login", user);
            var content = await response.Content.ReadAsStringAsync();
            var userObj = JsonConvert.DeserializeObject<ApiUser>(content);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userObj.Token);
            return userObj;
        }
        protected void Logout()
        {
            client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
