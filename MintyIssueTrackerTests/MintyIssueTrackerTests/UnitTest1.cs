using Bogus;
using MintyIssueTrackerTests.Interfaces;
using MintyIssueTrackerTests.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Threading.Tasks;

namespace MintyIssueTrackerTests
{
    public class Tests
    {
        private IBuilder _endpointBuilder;
        private Faker _bogus = new Faker();

        private static UserModel _userCredentials;
        private static string _token;
        private static long _jobId;

        [SetUp]
        public void Setup()
        {
            _endpointBuilder = new EndpointBuilder();
        }

        private bool IsValidJSONSchema(string jsonSchema, string json)
        {
            JsonSchema schema = JsonSchema.Parse(jsonSchema);
            var data = JObject.Parse(json);
            return data.IsValid(schema);
        }

        [Test, Order(0)]
        public async Task RegistrationUser_CorrectData_Success()
        {
            var jsonSchema = @"{
                    'type': 'object',
                    'properties': {
                        'username': {'type': 'string'},
                        'firstname': {'type': 'string'},
                        'lastname': {'type': 'string'}
                    }
                 }";

            var body = new {
                username = _bogus.Name.LastName(),
                password = _bogus.Name.Random.String(10),
                firstname = _bogus.Name.FirstName(),
                lastname = _bogus.Name.LastName()
            };
            _userCredentials = new UserModel() 
            {
                Username = body.username,
                Password = body.password
            };

            var _client = _endpointBuilder.BuildRegistrationEndpoint();

            var request = new RestRequest()
                .AddJsonBody(body)
                .AddQueryParameter("roleName", "admin");

            var response = await _client.ExecutePostAsync(request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }
        [Test, Order(1)]
        public async Task RegistrationUser_InvalidData_Failed()
        {
            var body = new
            {
                username = _bogus.Name.Random.String(2),
                password = _bogus.Name.Random.String(2),
                firstname = _bogus.Name.FirstName(),
                lastname = _bogus.Name.LastName()
            };

            var _client = _endpointBuilder.BuildRegistrationEndpoint();
            var request = new RestRequest()
                .AddJsonBody(body)
                .AddQueryParameter("roleName", "admin");

            var response = await _client.ExecutePostAsync(request);
            var x = response.StatusCode;
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }
        [Test, Order(2)]
        public async Task GetAccessToken_CorrectData_Success()
        {
            var jsonSchema = @"{
                    'type': 'object',
                    'properties': {
                        'access_token': {'type': 'string'}
                    }
                 }";

            var _client = _endpointBuilder.BuildTokenEndpoint();
            var request = new RestRequest().AddJsonBody(_userCredentials);
            var response = await _client.ExecutePostAsync(request);

            dynamic tmp = JsonConvert.DeserializeObject(response.Content);
            _token = (string)tmp.access_token;

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }
        [Test, Order(3)]
        public async Task GetAccessToken_FakeData_Success()
        {
            var fakeUser = new
            {
                username = _bogus.Name.Random.String(7),
                password = _bogus.Name.Random.String(10),
            };
            var _client = _endpointBuilder.BuildTokenEndpoint();
            var request = new RestRequest().AddJsonBody(fakeUser);
            var response = await _client.ExecutePostAsync(request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.Conflict);
        }
        [Test, Order(4)]
        public async Task GetAccessToken_InvalidData_Success()
        {
            var invalidUser = new
            {
                username = _bogus.Name.Random.String(3),
                password = _bogus.Name.Random.String(3),
            };
            var _client = _endpointBuilder.BuildTokenEndpoint();
            var request = new RestRequest().AddJsonBody(invalidUser);
            var response = await _client.ExecutePostAsync(request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }
        [Test,Order(5)]
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
            var _client = _endpointBuilder.BuildUserInfoEndpoint();
            var request = new RestRequest().AddHeader("Authorization", ("Bearer " + _token));
            var response = await _client.ExecuteGetAsync(request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }
        [Test, Order(6)]
        public async Task CreateJob_CorrectData_Success()
        {
            var jsonSchema = @"{
                    'type': 'object',
                    'properties': {
                        'jobId': {'type': 'number'}
                      }
                 }";
            var job = new
            {
                name = _bogus.Name.JobTitle(),
                description = _bogus.Name.JobDescriptor(),
                timeSchedule = _bogus.Date.FutureOffset()
            };
            var _client = _endpointBuilder.BuildCreateJobEndpoint();
            var request = new RestRequest().AddHeader("Authorization", ("Bearer " + _token)).AddJsonBody(job);

            var response = await _client.ExecutePostAsync(request);

            dynamic tmp = JsonConvert.DeserializeObject(response.Content);
            _jobId = (long)tmp.jobId;

            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }
        [Test, Order(7)]
        public async Task CreateJob_InvalidTime_Success()
        {
            var job = new
            {
                name = _bogus.Name.JobTitle(),
                description = _bogus.Name.JobDescriptor(),
                timeSchedule = _bogus.Date.Past()
            };
            var _client = _endpointBuilder.BuildCreateJobEndpoint();
            var request = new RestRequest().AddHeader("Authorization", ("Bearer " + _token)).AddJsonBody(job);
            var response = await _client.ExecutePostAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }
        [Test, Order(8)]
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
            var _client = _endpointBuilder.BuildGetJobEndpoint();
            var request = new RestRequest().AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", _jobId);
            var response = await _client.ExecuteGetAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, JsonConvert.DeserializeObject(response.Content).ToString()));
        }
        [Test, Order(9)]
        public async Task GetJob_NonexistentData_Success()
        {
            var _client = _endpointBuilder.BuildGetJobEndpoint();
            var request = new RestRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", long.MaxValue);
            var response = await _client.ExecuteGetAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }
        [Test, Order(10)]
        public async Task GetJob_InvalidData_Success()
        {
            var _client = _endpointBuilder.BuildGetJobEndpoint();
            var request = new RestRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", -1);
            var response = await _client.ExecuteGetAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }
        [Test, Order(11)]
        public async Task GetListJobs_CorrectData_Success()
        {
            var userId = new Repository().GetIdByUsername(_userCredentials.Username);
            var _client = _endpointBuilder.BuildGetListJobsEndpoint();
            var request = new RestRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("userId", userId);
            var response = await _client.GetAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
        [Test, Order(12)]
        public async Task CancelJob_CorrectData_Success()
        {
            var _client = _endpointBuilder.BuildCancelJobEndpoint();
            var request = new RestRequest().AddHeader("Authorization", ("Bearer " + _token)).AddQueryParameter("jobId", _jobId);
            var response = await _client.ExecutePutAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
        [Test, Order(13)]
        public async Task CancelJob_InvalidData_Success()
        {
            var _client = _endpointBuilder.BuildCancelJobEndpoint();
            var request = new RestRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", -1);
            var response = await _client.ExecutePutAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }
        [Test, Order(14)]
        public async Task UpdateJob_CorrectData_Success()
        {
            var job = new
            {
                name = _bogus.Name.JobTitle(),
                description = _bogus.Name.JobDescriptor(),
                timeSchedule = _bogus.Date.FutureOffset()
            };
            var _client = _endpointBuilder.BuildCancelJobEndpoint();
            var request = new RestRequest().AddHeader("Authorization", ("Bearer " + _token)).AddQueryParameter("jobId", _jobId).AddJsonBody(job);
            var response = await _client.ExecutePutAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
        [Test, Order(15)]
        public async Task UpdateJob_InvalidId_Success()
        {
            var job = new
            {
                name = _bogus.Name.JobTitle(),
                description = _bogus.Name.JobDescriptor(),
                timeSchedule = _bogus.Date.FutureOffset()
            };
            var _client = _endpointBuilder.BuildCancelJobEndpoint();
            var request = new RestRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", -1)
                .AddJsonBody(job);
            var response = await _client.ExecutePutAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }
        [Test, Order(16)]
        public async Task UploadImagesForJob_CorrectData_Success()
        {
            var jsonSchema = @"{
                    'type': 'object',
                    'properties': {
                        'Image': {'type': 'string'}
                      }
                 }";

            var _client = _endpointBuilder.BuildUploadImagesForJobEndpoint();
            var request = new RestRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddHeader("Content-Type", "multipart/form-data")
                .AddQueryParameter("jobId", _jobId)
                .AddQueryParameter("width", _bogus.Random.Int(100, 2000))
                .AddQueryParameter("height", _bogus.Random.Int(100, 2000))
                .AddFile("httpRequest", @"C:\Users\i_moroz\Desktop\a.jpg");

            var response = await _client.ExecutePostAsync(request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }
        [Test, Order(17)]
        public async Task UploadAvatar_CorrectData_Success()
        {
            var _client = _endpointBuilder.BuildUploadAvatarEndpoint();
            var request = new RestRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddHeader("Content-Type", "multipart/form-data")
                .AddFile("file", @"C:\Users\i_moroz\Desktop\a.jpg");
            var response = await _client.ExecutePostAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);
        }
        [Test, Order(18)]
        public async Task UploadAvatar_EmptyImage_Success()
        {
            var _client = _endpointBuilder.BuildUploadAvatarEndpoint();
            
            var request = new RestRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddHeader("Content-Type", "multipart/form-data");

            var response = await _client.ExecutePostAsync(request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }
        [Test, Order(19)]
        public async Task DeleteUser_CorrectData_Success()
        {
            var userId = new Repository().GetIdByUsername(_userCredentials.Username);
            var _client = _endpointBuilder.BuildDeleteUserEndpoint();
            var request = new RestRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("id", userId);
            var response = await _client.DeleteAsync(request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
        
    }
}