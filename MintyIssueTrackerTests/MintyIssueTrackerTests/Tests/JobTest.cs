using Bogus;
using MintyIssueTrackerTests.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using MintyIssueTrackerTests.Directors;
using MintyIssueTrackerTests.Entity;
using MintyIssueTrackerTests.Logger;
using System;

namespace MintyIssueTrackerTests.Tests
{
    [TestFixture]
    public class JobTest : RequestLogger
    {
        private EndpointBuilder _endpointBuilder;
        private Faker _bogus;

        private static UserModel _userCredentials;
        private string _token;
        private long _jobId;

        [SetUp]
        public async Task TestSetup()
        {
            _bogus = new Faker();
            _endpointBuilder = new EndpointBuilder();
            _userCredentials = await AuthenticationDirector.CreateCredentials();
            _token = await AuthenticationDirector.GetToken(_userCredentials);
            _jobId = await JobDirector.GetJobId(_token);
        }


        [Test, Description("Create job with correct data")]
        [Category("Create job")]
        public async Task CreateJob_CorrectData_Success()
        {
            WriteToLog("Create job with correct data");
            var job = new JobModel
            {
                Name = _bogus.Name.JobTitle(),
                Description = _bogus.Name.JobDescriptor(),
                TimeSchedule = _bogus.Date.Future()
            };

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildCreateJobEndpoint())
                .SetToken(_token)
                .SetBody(job)
                .SendRequest();

            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Assert.IsTrue(JSONHelper.IsValidJSONSchema(JSONHelper.CreateJobResponseSchemaPath, response.Content));
            });
            
        }


        [Test, Description("Create job with past time")]
        [Category("Create job")]
        public async Task CreateJob_InvalidTime_Failed()
        {
            WriteToLog("Create job with past time");
            var job = new JobModel()
            {
                Name = _bogus.Name.JobTitle(),
                Description = _bogus.Name.JobDescriptor(),
                TimeSchedule = _bogus.Date.Past()
            };

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildCreateJobEndpoint())
                .SetToken(_token)
                .SetBody(job)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Test, Description("Get job by id")]
        [Category("Get job")]
        public async Task GetJob_CorrectData_Success()
        {
            WriteToLog("Get job by id");
            

            var response = await RequestFactory
                .RequestManager
                .CreateGetRequest()
                .SetApiEndpoint(_endpointBuilder.BuildGetJobEndpoint())
                .SetToken(_token)
                .AddQueryParameter("jobId", _jobId)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsTrue(JSONHelper.IsValidJSONSchema(JSONHelper.GetJobResponseSchemaPath, JsonConvert.DeserializeObject(response.Content).ToString()));
            });
        }


        [Test, Description("Get job by nonexistent id")]
        [Category("Get job")]
        public async Task GetJob_NonexistentData_Failed()
        {
            WriteToLog("Get job by nonexistent id");
            var response = await RequestFactory
                .RequestManager
                .CreateGetRequest()
                .SetApiEndpoint(_endpointBuilder.BuildGetJobEndpoint())
                .SetToken(_token)
                .AddQueryParameter("jobId", long.MaxValue)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        
        
        [Test, Description("Get job by invalid id")]
        [Category("Get job")]
        public async Task GetJob_InvalidData_Failed()
        {
            WriteToLog("Get job by invalid id");
            var response = await RequestFactory
                .RequestManager
                .CreateGetRequest()
                .SetApiEndpoint(_endpointBuilder.BuildGetJobEndpoint())
                .SetToken(_token)
                .AddQueryParameter("jobId", -1)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        
        [Test, Description("Get list of jobs by user id")]
        [Category("Get job")]
        public async Task GetListJobs_CorrectData_Success()
        {
            WriteToLog("Get list of jobs by user id");
            var userId = Repository.GetByKey<User>("Username", _userCredentials.Username).Id;
            
            var response = await RequestFactory
                .RequestManager
                .CreateGetRequest()
                .SetApiEndpoint(_endpointBuilder.BuildGetListJobsEndpoint())
                .SetToken(_token)
                .AddQueryParameter("userId", userId)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        
        
        [Test, Description("Cancel job by job id")]
        [Category("Cancel job")]
        public async Task CancelJob_CorrectData_Success()
        {
            WriteToLog("Cancel job by job id");
            var response = await RequestFactory
                .RequestManager
                .CreatePutRequest()
                .SetApiEndpoint(_endpointBuilder.BuildCancelJobEndpoint())
                .SetToken(_token)
                .AddQueryParameter("jobId", _jobId)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        
        
        [Test, Description("Cancel job by invalid id")]
        [Category("Cancel job")]
        public async Task CancelJob_InvalidData_Failed()
        {
            WriteToLog("Cancel job by invalid id");
            var response = await RequestFactory
                .RequestManager
                .CreatePutRequest()
                .SetApiEndpoint(_endpointBuilder.BuildCancelJobEndpoint())
                .SetToken(_token)
                .AddQueryParameter("jobId", -1)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        
        [Test, Description("Update job information")]
        [Category("Update job")]
        public async Task UpdateJob_CorrectData_Success()
        {
            WriteToLog("Update job information");
            var job = new JobModel
            {
                Name = _bogus.Name.JobTitle(),
                Description = _bogus.Name.JobDescriptor(),
                TimeSchedule = _bogus.Date.Future()
            };

            var response = await RequestFactory
                .RequestManager
                .CreatePutRequest()
                .SetApiEndpoint(_endpointBuilder.BuildUpdateJobEndpoint())
                .SetToken(_token)
                .SetBody(job)
                .AddQueryParameter("jobId", _jobId)
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        
        
        [Test, Description("Update job information by invalid id")]
        [Category("Update job")]
        public async Task UpdateJob_InvalidId_Failed()
        {
            WriteToLog("Update job information by invalid id");
            var job = new
            {
                name = _bogus.Name.JobTitle(),
                description = _bogus.Name.JobDescriptor(),
                timeSchedule = _bogus.Date.FutureOffset()
            };

            var response = await RequestFactory
                .RequestManager
                .CreatePutRequest()
                .SetApiEndpoint(_endpointBuilder.BuildUpdateJobEndpoint())
                .SetToken(_token)
                .SetBody(job)
                .AddQueryParameter("jobId", -1)
                .SendRequest();

            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        
        [Test, Description("Upload images for job")]
        [Category("Upload image")]
        public async Task UploadImagesForJob_CorrectData_Success()
        {
            WriteToLog("Upload images for job");

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildUploadImagesForJobEndpoint())
                .SetFormData()
                .SetToken(_token)
                .AddQueryParameter("jobId", _jobId)
                .AddQueryParameter("width", _bogus.Random.Int(100, 2000))
                .AddQueryParameter("height", _bogus.Random.Int(100, 2000))
                .AddFile("httpRequest", $"{Environment.CurrentDirectory}\\Images\\a.jpg")
                .SendRequest();
            WriteToLog("Response status code: " + response.StatusCode.ToString() + " Response body: " + response.Content);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsTrue(JSONHelper.IsValidJSONSchema(JSONHelper.UploadImageResponseSchemaPath, response.Content));
            });
        }
    }
}
