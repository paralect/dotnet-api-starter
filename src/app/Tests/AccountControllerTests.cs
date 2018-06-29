using Api.Core.Abstract;
using Api.Models.Account;
using Microsoft.AspNetCore.Mvc.Testing;
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
    public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        private readonly SignupModel _testSignupData;
        private readonly SigninModel _testSigninData;

        public AccountControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;

            _testSignupData = new SignupModel
            {
                FirstName = "Ivan",
                LastName = "Balalaikin",
                Email = "test@test.test",
                Password = "qwerty",
            };

            _testSigninData = new SigninModel
            {
                Email = _testSignupData.Email,
                Password = _testSignupData.Password,
            };
        }

        [Fact]
        public async Task SignupSuccessful()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();


            HttpResponseMessage response = await client.PostAsJsonAsync("/account/signup", _testSignupData);


            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task SignupReturnErrorEmailAlreadyRegistered()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();


            await client.PostAsJsonAsync("/account/signup", _testSignupData);
            HttpResponseMessage response = await client.PostAsJsonAsync("/account/signup", _testSignupData);


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("User with this email is already registered", responseContent);
        }

        [Fact]
        public async Task VerifyEmailReturnErrorInvalidSignupToken()
        {
            HttpClient client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            _factory.DropDatabase();


            HttpResponseMessage response = await client.GetAsync("/account/verifyEmail/111");


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Token is invalid", responseContent);
        }

        [Fact]
        public async Task VerifyEmailSuccessful()
        {
            HttpClient client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            _factory.DropDatabase();
            var userRepository = _factory.GetService<IUserRepository>();


            await client.PostAsJsonAsync("/account/signup", _testSignupData);

            string signupToken = userRepository.FindOne(x => x.Email == _testSignupData.Email).SignupToken;
            HttpResponseMessage response = await client.GetAsync($"/account/verifyEmail/{signupToken}");


            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        }

        [Fact]
        public async Task SigninReturnErrorIncorrectCredentials()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();


            HttpResponseMessage response = await client.PostAsJsonAsync("/account/signin", new SigninModel
            {
                Email = "wrong@e.mail",
                Password = "wrongPassword"
            });


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Incorrect email or password", responseContent);
        }

        [Fact]
        public async Task SigninReturnErrorEmailIsNotVerified()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();


            await client.PostAsJsonAsync("/account/signup", _testSignupData);
            HttpResponseMessage response = await client.PostAsJsonAsync("/account/signin", _testSigninData);


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Please verify your email to sign in", responseContent);
        }

        [Fact]
        public async Task SigninSuccessful()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();
            var userRepository = _factory.GetService<IUserRepository>();


            await client.PostAsJsonAsync("/account/signup", _testSignupData);

            string signupToken = userRepository.FindOne(x => x.Email == _testSignupData.Email).SignupToken;
            await client.GetAsync($"/account/verifyEmail/{signupToken}");

            HttpResponseMessage response = await client.PostAsJsonAsync("/account/signin", _testSigninData);


            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ForgotPasswordReturnErrorEmailInvalid()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();


            HttpResponseMessage response = await client.PostAsJsonAsync("/account/forgotPassword", new ForgotPasswordModel { Email = "wrong" });


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Please enter a valid email address", responseContent);
        }

        [Fact]
        public async Task ForgotPasswordReturnErrorEmailIsNotFound()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();


            HttpResponseMessage response = await client.PostAsJsonAsync("/account/forgotPassword", new ForgotPasswordModel { Email = "any@e.mail" });


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Couldn't find account associated with any@e.mail. Please try again.", responseContent);
        }

        [Fact]
        public async Task ForgotPasswordSuccessful()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();
            var userRepository = _factory.GetService<IUserRepository>();


            await client.PostAsJsonAsync("/account/signup", _testSignupData);

            string signupToken = userRepository.FindOne(x => x.Email == _testSignupData.Email).SignupToken;
            await client.GetAsync($"/account/verifyEmail/{signupToken}");

            HttpResponseMessage response = await client.PostAsJsonAsync("/account/forgotPassword", new ForgotPasswordModel { Email = _testSigninData.Email });


            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResetPasswordReturnErrorResetPasswordTokenInvalid()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();


            HttpResponseMessage response = await client.PostAsJsonAsync("/account/resetPassword", new ResetPasswordModel
            {
                Password = "anyPassword",
                Token = "wrongToken"
            });


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Password reset link has expired or invalid", responseContent);
        }

        [Fact]
        public async Task ResetPasswordSuccessful()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();
            var userRepository = _factory.GetService<IUserRepository>();


            await client.PostAsJsonAsync("/account/signup", _testSignupData);

            string signupToken = userRepository.FindOne(x => x.Email == _testSignupData.Email).SignupToken;
            await client.GetAsync($"/account/verifyEmail/{signupToken}");

            await client.PostAsJsonAsync("/account/forgotPassword", new ForgotPasswordModel { Email = _testSigninData.Email });

            string resetPasswordToken = userRepository.FindOne(x => x.Email == _testSignupData.Email).ResetPasswordToken;
            HttpResponseMessage response = await client.PostAsJsonAsync("/account/resetPassword", new ResetPasswordModel
            {
                Password = "newPassword",
                Token = resetPasswordToken
            });


            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ResendVerificationEmailSuccessful()
        {
            HttpClient client = _factory.CreateClient();
            _factory.DropDatabase();


            HttpResponseMessage response = await client.PostAsJsonAsync("/account/resendVerification", new ResendVerificationModel
            {
                Email = "any@e.mail"
            });


            response.EnsureSuccessStatusCode();
        }
    }
}
