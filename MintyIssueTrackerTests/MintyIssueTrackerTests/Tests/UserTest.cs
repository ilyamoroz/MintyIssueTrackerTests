using MintyIssueTrackerTests.Directors;
using MintyIssueTrackerTests.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Threading.Tasks;

namespace MintyIssueTrackerTests.Tests
{
    [TestFixture]
    public class UserTest
    {
        private EndpointBuilder _endpointBuilder;

        private static UserModel _userCredentials;
        private static string _token;
        
        [OneTimeSetUp]
        public void TestSetup()
        {
            _endpointBuilder = new EndpointBuilder();
        }
        [SetUp]
        public async Task Setup()
        {
            _userCredentials = await AuthenticationDirector.CreateCredentials();
            _token = await AuthenticationDirector.GetToken(_userCredentials);
        }

        private bool IsValidJSONSchema(string jsonSchema, string json)
        {
            JsonSchema schema = JsonSchema.Parse(jsonSchema);
            var data = JObject.Parse(json);
            return data.IsValid(schema);
        }

        [Test]
        public async Task GetUserInfo()
        {
            var jsonSchema = @"{
                    'type': 'object',
                    'properties': {
                        'username': {'type': 'string'},
                        'password': {'type': 'string'},
                        'firstname': {'type': 'string'},
                        'lastname': {'type': 'string'},
                         'role': {'type': 'string'}
                      }
                 }";

            var response = await RequestFactory
                .RequestManager
                .CreateGetRequest()
                .SetApiEndpoint(_endpointBuilder.BuildUserInfoEndpoint())
                .SetToken(_token)
                .SendRequest();
                
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }

        [Test]
        public async Task DeleteUser_CorrectData_Success()
        {
            var userId = new Repository().GetIdByUsername(_userCredentials.Username);

            var response = await RequestFactory
                .RequestManager
                .CreateDeleteRequest()
                .SetApiEndpoint(_endpointBuilder.BuildDeleteUserEndpoint())
                .SetToken(_token)
                .AddQueryParameter("id", userId)
                .SendRequest();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
