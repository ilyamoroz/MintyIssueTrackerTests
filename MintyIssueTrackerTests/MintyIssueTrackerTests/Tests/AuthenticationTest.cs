using Bogus;
using MintyIssueTrackerTests.Directors;
using MintyIssueTrackerTests.Model;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using MintyIssueTrackerTests.Logger;

namespace MintyIssueTrackerTests.Tests
{
    public class AuthenticationTest : RequestLogger
    {
        private EndpointBuilder _endpointBuilder;
        private Faker _bogus;
        private static UserModel _userCredentials;
        
        [SetUp]
        public async Task TestSetup()
        {
            _bogus = new Faker();
            _endpointBuilder = new EndpointBuilder();
            _userCredentials = await AuthenticationDirector.CreateCredentials();
        }

        [Test, Description("Registration user with correct data")]
        [Category("Registration")]
        public async Task RegistrationUser_CorrectData_Success()
        {
            WriteToLog("Register User correct data");
            

            var body = new CreateUserModel()
            {
                Username = _bogus.Random.String2(minLength: 5, maxLength: 16),
                Password = _bogus.Random.String2(minLength: 7, maxLength: 16),
                Firstname = _bogus.Name.FirstName(),
                Lastname = _bogus.Name.LastName()
            };

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildRegistrationEndpoint())
                .SetBody(body)
                .AddQueryParameter("roleName", "admin")
                .SendRequest();

            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, response.Content);
                Assert.IsTrue(JSONHelper.IsValidJSONSchema(JSONHelper.RegisterUserResponseSchemaPath, response.Content));
            });
            

        }


        [Test, Description("Registration user with invalid data")]
        [Category("Registration")]
        public async Task RegistrationUser_InvalidData_Failed()
        {
            WriteToLog("Register User invalid data");
            var body = new CreateUserModel()
            {
                Username = _bogus.Random.String2(length: 3),
                Password = _bogus.Random.String2(length: 3),
                Firstname = _bogus.Name.FirstName(),
                Lastname = _bogus.Name.LastName()
            };
            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildRegistrationEndpoint())
                .SetBody(body)
                .AddQueryParameter("roleName", "admin")
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, response.Content);
        }


        [Test, Description("Get access token with exist user")]
        [Category("Access token")]
        public async Task GetAccessToken_CorrectData_Success()
        {
            WriteToLog("Get access token correct data");

            

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildTokenEndpoint())
                .SetBody(_userCredentials)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, response.Content);
                Assert.IsTrue(JSONHelper.IsValidJSONSchema(JSONHelper.GetAccessTokenResponseSchemaPath, response.Content));
            });
        }


        [Test, Description("Get access token with fake user")]
        [Category("Access token")]
        public async Task GetAccessToken_FakeData_Failed()
        {
            WriteToLog("Get access token with fake user");
            var fakeUser = new UserModel
            {
                Username = _bogus.Random.String2(minLength: 5, maxLength: 16),
                Password = _bogus.Random.String2(minLength: 7, maxLength: 16)
            };

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildTokenEndpoint())
                .SetBody(fakeUser)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode, response.Content);
        }


        [Test, Description("Get access token with invalid data")]
        [Category("Access token")]
        public async Task GetAccessToken_InvalidData_Failed()
        {
            var invalidUser = new UserModel
            {
                Username = _bogus.Name.Random.String(3),
                Password = _bogus.Name.Random.String(3),
            };
            WriteToLog("Get access token with invalid user");
            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildTokenEndpoint())
                .SetBody(invalidUser)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, response.Content);
        }
    }
}
