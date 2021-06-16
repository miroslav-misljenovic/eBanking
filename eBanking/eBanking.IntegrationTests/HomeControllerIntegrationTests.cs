using Xunit;
using System.Net.Http;

namespace eBanking.IntegrationTests
{
    public class HomeControllerIntegrationTests : IClassFixture<TestingWebAppFactory<Startup>>
    {
        private readonly HttpClient _client;
        public HomeControllerIntegrationTests(TestingWebAppFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public void LoginTest()
        {
            //var response = _client.GetAsync("http://liss.matf.bg.ac.rs:5000/Identity/Account/Login").Result;
            var response = _client.GetAsync("http://62.75.156.53:5000/Identity/Account/Login").Result;

            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;

            Assert.Contains("Forgot your password?", responseString);
        }

        [Fact]
        public void HomeTest()
        {
        
            //var response = _client.GetAsync("http://liss.matf.bg.ac.rs:5000/").Result;
            var response = _client.GetAsync("http://62.75.156.53:5000/").Result;
            
            response.EnsureSuccessStatusCode();

            var responseString = response.Content.ReadAsStringAsync().Result;

            Assert.Contains("Currency Explorer", responseString);
            Assert.Contains("RSD", responseString);
            Assert.Contains("HKD", responseString);
            Assert.Contains("BRL", responseString);
            Assert.Contains("JPY", responseString);
            Assert.Contains("RUB", responseString);
        }
    }
}
