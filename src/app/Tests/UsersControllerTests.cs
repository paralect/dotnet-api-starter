using Api.Core.Abstract;
using Api.Models.Account;
using Api.Models.User;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tests.Setup;
using Xunit;

namespace Tests
{
    public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        private readonly SignupModel _testSignupData;

        public UsersControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;

            _testSignupData = new SignupModel
            {
                FirstName = "Ivan",
                LastName = "Balalaikin",
                Email = "test@test.test",
                Password = "qwerty",
            };
        }

        [Theory]
        [InlineData("current", "get")]
        [InlineData("current", "put")]
        public async Task ActionsRequireAuthorization(string url, string method)
        {
            HttpClient client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "wrongAuthToken");
            _factory.DropDatabase();


            HttpResponseMessage response = null;
            switch (method)
            {
                case "get":
                    response = await client.GetAsync($"/users/{url}");
                    break;
                case "post":
                    response = await client.PostAsJsonAsync($"/users/{url}", new { });
                    break;
                case "put":
                    response = await client.PutAsJsonAsync($"/users/{url}", new { });
                    break;
            }


            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrentReturnCorrectData()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();
            var authService = _factory.GetService<IAuthService>();
            var userRepository = _factory.GetService<IUserRepository>();


            await client.PostAsJsonAsync("/account/signup", _testSignupData);

            ObjectId userId = userRepository.FindOne(x => x.Email == _testSignupData.Email).Id;
            string authToken = authService.CreateAuthToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            HttpResponseMessage response = await client.GetAsync("/users/current");


            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains(userId.ToString(), responseContent);
        }

        [Fact]
        public async Task UpdateUserInfoReturnErrorAlreadyInUse()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();
            var authService = _factory.GetService<IAuthService>();
            var userRepository = _factory.GetService<IUserRepository>();

            var secondSignupData = new SignupModel
            {
                FirstName = "Petr",
                LastName = "Ivanov",
                Email = "petr@ivanov.test",
                Password = "qwerty",
            };


            await client.PostAsJsonAsync("/account/signup", _testSignupData);
            await client.PostAsJsonAsync("/account/signup", secondSignupData);

            ObjectId userId = userRepository.FindOne(x => x.Email == _testSignupData.Email).Id;
            string authToken = authService.CreateAuthToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            HttpResponseMessage response = await client.PutAsJsonAsync("/users/current", new UpdateCurrentModel
            {
                FirstName = _testSignupData.FirstName,
                LastName = _testSignupData.LastName,
                Email = secondSignupData.Email
            });


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("This email is already in use", responseContent);
        }

        [Fact]
        public async Task SuccessfullyUpdateUserInfo()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();
            var authService = _factory.GetService<IAuthService>();
            var userRepository = _factory.GetService<IUserRepository>();
            

            await client.PostAsJsonAsync("/account/signup", _testSignupData);

            ObjectId userId = userRepository.FindOne(x => x.Email == _testSignupData.Email).Id;
            string authToken = authService.CreateAuthToken(userId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            HttpResponseMessage response = await client.PutAsJsonAsync("/users/current", new UpdateCurrentModel
            {
                FirstName = _testSignupData.FirstName,
                LastName = _testSignupData.LastName,
                Email = "new@e.mail"
            });


            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains(userId.ToString(), responseContent);
        }
    }
}
