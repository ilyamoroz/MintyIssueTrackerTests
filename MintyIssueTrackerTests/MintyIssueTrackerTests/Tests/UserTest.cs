using MintyIssueTrackerTests.Directors;
using MintyIssueTrackerTests.Entity;
using MintyIssueTrackerTests.Model;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using MintyIssueTrackerTests.Logger;

namespace MintyIssueTrackerTests.Tests
{
    [TestFixture]
    public class UserTest : RequestLogger
    {
        private EndpointBuilder _endpointBuilder;

        private static UserModel _userCredentials;
        private string _token;
        
        [SetUp]
        public async Task Setup()
        {
            _endpointBuilder = new EndpointBuilder();
            _userCredentials = await AuthenticationDirector.CreateCredentials();
            _token = await AuthenticationDirector.GetToken(_userCredentials);
        }
        

        [Test, Description("Get information about user")]
        [Category("User")]
        public async Task GetUserInfo()
        {
            WriteToLog("Get information about user");
           

            var response = await RequestFactory
                .RequestManager
                .CreateGetRequest()
                .SetApiEndpoint(_endpointBuilder.BuildUserInfoEndpoint())
                .SetToken(_token)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, response.Content);
                Assert.IsTrue(JSONHelper.IsValidJSONSchema(JSONHelper.GetUserResponseSchemaPath, response.Content));
            });
        }


        [Test, Description("Delete user by id")]
        [Category("User")]
        public async Task DeleteUser_CorrectData_Success()
        {
            WriteToLog("Delete user by id");
            var userId = Repository.GetByKey<User>("Username", _userCredentials.Username).Id;

            var response = await RequestFactory
                .RequestManager
                .CreateDeleteRequest()
                .SetApiEndpoint(_endpointBuilder.BuildDeleteUserEndpoint())
                .SetToken(_token)
                .AddQueryParameter("id", userId)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, response.Content);
        }
    }
}
