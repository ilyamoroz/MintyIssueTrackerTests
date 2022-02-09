using MintyIssueTrackerTests.Directors;
using MintyIssueTrackerTests.Model;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using MintyIssueTrackerTests.Logger;
using System;

namespace MintyIssueTrackerTests.Tests
{
    [TestFixture]
    public class ImageTest : RequestLogger
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

        [Test, Description("Upload avatar for user")]
        [Category("Upload avatar")]
        public async Task UploadAvatar_CorrectData_Success()
        {
            WriteToLog("Upload avatar for user");
            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildUploadAvatarEndpoint())
                .SetFormData()
                .SetToken(_token)
                .AddFile("file", $"{Environment.CurrentDirectory}\\Images\\a.jpg")
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);

            
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, response.Content);
        }
        [Test, Description("Upload empty image")]
        [Category("Upload avatar")]
        public async Task UploadAvatar_EmptyImage_Failed()
        {
            WriteToLog("Upload empty image");
            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildUploadAvatarEndpoint())
                .SetFormData()
                .SetToken(_token)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, response.Content);
        }
    }
}
