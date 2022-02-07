using Bogus;
using MintyIssueTrackerTests.Directors;
using MintyIssueTrackerTests.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
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

        private bool IsValidJSONSchema(string jsonSchema, string json)
        {
            JsonSchema schema = JsonSchema.Parse(jsonSchema);
            var data = JObject.Parse(json);
            return data.IsValid(schema);
        }

        [Test, Description("Registration user with correct data")]
        [Category("Registration")]
        public async Task RegistrationUser_CorrectData_Success()
        {
            WriteToLog("RegisterUser correct data");
            var jsonSchema = @"{
                    'type': 'object',
                    'properties': {
                        'username': {'type': 'string'},
                        'firstname': {'type': 'string'},
                        'lastname': {'type': 'string'}
                    }
                 }";

            var body = new CreateUserModel()
            {
                username = _bogus.Random.String2(minLength: 5, maxLength: 16),
                password = _bogus.Random.String2(minLength: 7, maxLength: 16),
                firstname = _bogus.Name.FirstName(),
                lastname = _bogus.Name.LastName()
            };

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildRegistrationEndpoint())
                .SetBody(body)
                .AddQueryParameter("roleName", "admin")
                .SendRequest();
            WriteToLog(response.StatusCode.ToString());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }

        [Test, Description("Registration user with invalid data")]
        [Category("Registration")]
        public async Task RegistrationUser_InvalidData_Failed()
        {
            WriteToLog("Register User invalid data");
            var body = new CreateUserModel()
            {
                username = _bogus.Random.String2(length: 3),
                password = _bogus.Random.String2(length: 3),
                firstname = _bogus.Name.FirstName(),
                lastname = _bogus.Name.LastName()
            };
            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildRegistrationEndpoint())
                .SetBody(body)
                .AddQueryParameter("roleName", "admin")
                .SendRequest();
            WriteToLog(response.StatusCode.ToString());
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test, Description("Get access token with exist user")]
        [Category("Access token")]
        public async Task GetAccessToken_CorrectData_Success()
        {
            WriteToLog("Get access token correct data");

            var jsonSchema = @"{
                    'type': 'object',
                    'properties': {
                        'access_token': {'type': 'string'}
                    }
                 }";

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildTokenEndpoint())
                .SetBody(_userCredentials)
                .SendRequest();
            WriteToLog(response.StatusCode.ToString());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
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
            WriteToLog(response.StatusCode.ToString());
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
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
            WriteToLog(response.StatusCode.ToString());
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
