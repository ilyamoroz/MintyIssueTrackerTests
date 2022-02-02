using Bogus;
using MintyIssueTrackerTests.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using MintyIssueTrackerTests.Directors;

namespace MintyIssueTrackerTests.Tests
{
    [TestFixture]
    public class JobTest
    {
        private EndpointBuilder _endpointBuilder;
        private Faker _bogus;

        private static UserModel _userCredentials;
        private static string _token;
        private static long _jobId;

        [OneTimeSetUp]
        public async Task TestSetup()
        {
            _bogus = new Faker();
            _endpointBuilder = new EndpointBuilder();
            _requestManager = new RequestManager();
            _userCredentials = await AuthenticationDirector.CreateCredentials();
        }
        [SetUp]
        public async Task Setup()
        {
            _token = await AuthenticationDirector.GetToken(_userCredentials);
            _jobId = await JobDirector.GetJobId(_token);
        }

        private bool IsValidJSONSchema(string jsonSchema, string json)
        {
            JsonSchema schema = JsonSchema.Parse(jsonSchema);
            var data = JObject.Parse(json);
            return data.IsValid(schema);
        }

        [Test]
        public async Task CreateJob_CorrectData_Success()
        {
            var jsonSchema = @"{
                    'type': 'object',
                    'properties': {
                        'jobId': {'type': 'number'}
                      }
                 }";
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

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }
        [Test]
        public async Task CreateJob_InvalidTime_Failed()
        {
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

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Test]
        public async Task GetJob_CorrectData_Success()
        {
            var jsonSchema = @"{
                    'type': 'object',
                    'properties': {
                        'Name': {'type': 'string'},
                        'Description': {'type': 'string'},
                        'TimeSchedule': {'type': 'string'},
                        'UserId': {'type': 'number'},
                        'StatusId': {'type': 'number'}
                      }
                 }";

            var response = await RequestFactory
                .RequestManager
                .CreateGetRequest()
                .SetApiEndpoint(_endpointBuilder.BuildGetJobEndpoint())
                .SetToken(_token)
                .AddQueryParameter("jobId", _jobId)
                .SendRequest();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, JsonConvert.DeserializeObject(response.Content).ToString()));
        }
        [Test]
        public async Task GetJob_NonexistentData_Failed()
        {
            var response = await RequestFactory
                .RequestManager
                .CreateGetRequest()
                .SetApiEndpoint(_endpointBuilder.BuildGetJobEndpoint())
                .SetToken(_token)
                .AddQueryParameter("jobId", long.MaxValue)
                .SendRequest();

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        [Test]
        public async Task GetJob_InvalidData_Failed()
        {
            var response = await RequestFactory
                .RequestManager
                .CreateGetRequest()
                .SetApiEndpoint(_endpointBuilder.BuildGetJobEndpoint())
                .SetToken(_token)
                .AddQueryParameter("jobId", -1)
                .SendRequest();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Test]
        public async Task GetListJobs_CorrectData_Success()
        {
            var userId = new Repository().GetIdByUsername(_userCredentials.Username);
            
            var response = await RequestFactory
                .RequestManager
                .CreateGetRequest()
                .SetApiEndpoint(_endpointBuilder.BuildGetListJobsEndpoint())
                .SetToken(_token)
                .AddQueryParameter("userId", userId)
                .SendRequest();
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test]
        public async Task CancelJob_CorrectData_Success()
        {
            var response = await RequestFactory
                .RequestManager
                .CreatePutRequest()
                .SetApiEndpoint(_endpointBuilder.BuildCancelJobEndpoint())
                .SetToken(_token)
                .AddQueryParameter("jobId", _jobId)
                .SendRequest(); 
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test]
        public async Task CancelJob_InvalidData_Failed()
        {
            var response = await RequestFactory
                .RequestManager
                .CreatePutRequest()
                .SetApiEndpoint(_endpointBuilder.BuildCancelJobEndpoint())
                .SetToken(_token)
                .AddQueryParameter("jobId", -1)
                .SendRequest();
            
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Test]
        public async Task UpdateJob_CorrectData_Success()
        {
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

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test]
        public async Task UpdateJob_InvalidId_Failed()
        {
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
            

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Test]
        public async Task UploadImagesForJob_CorrectData_Success()
        {
            var jsonSchema = @"{
                    'type': 'object',
                    'properties': {
                        'Image': {'type': 'string'}
                      }
                 }";

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildUploadImagesForJobEndpoint())
                .SetFormData()
                .SetToken(_token)
                .AddQueryParameter("jobId", _jobId)
                .AddQueryParameter("width", _bogus.Random.Int(100, 2000))
                .AddQueryParameter("height", _bogus.Random.Int(100, 2000))
                .AddFile("httpRequest", @"C:\Users\i_moroz\Desktop\a.jpg")
                .SendRequest();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }
    }
}
