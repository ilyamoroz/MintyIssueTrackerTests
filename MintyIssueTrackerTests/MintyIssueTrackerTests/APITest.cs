using Bogus;
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
    [TestFixture]
    public class APITest
    {
        private EndpointBuilder _endpointBuilder;
        private Faker _bogus;
        private RequestManager _requestManager;

        private static UserModel _userCredentials;
        private static string _token;
        private static long _jobId;

        [OneTimeSetUp]
        public void Setup()
        {
            _bogus = new Faker();
            _endpointBuilder = new EndpointBuilder();
            _requestManager = new RequestManager();
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

            var body = new
            {
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

            var request = _requestManager.CreatePostRequest()
                .AddJsonBody(body)
                .AddQueryParameter("roleName", "admin");
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildRegistrationEndpoint(), request);

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

            var request = _requestManager.CreatePostRequest()
                .AddJsonBody(body)
                .AddQueryParameter("roleName", "admin");

            var response = await _requestManager.GetResponse(_endpointBuilder.BuildRegistrationEndpoint(), request);
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

            var request = _requestManager.CreatePostRequest()
                .AddJsonBody(_userCredentials);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildTokenEndpoint(), request);

            dynamic tmp = JsonConvert.DeserializeObject(response.Content);
            _token = (string)tmp.access_token;

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }
        [Test, Order(3)]
        public async Task GetAccessToken_FakeData_Failed()
        {
            var fakeUser = new
            {
                username = _bogus.Name.Random.String(7),
                password = _bogus.Name.Random.String(10),
            };

            var request = _requestManager.CreatePostRequest()
                .AddJsonBody(fakeUser);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildTokenEndpoint(), request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.Conflict);
        }
        [Test, Order(4)]
        public async Task GetAccessToken_InvalidData_Failed()
        {
            var invalidUser = new
            {
                username = _bogus.Name.Random.String(3),
                password = _bogus.Name.Random.String(3),
            };

            var request = _requestManager.CreatePostRequest()
                .AddJsonBody(invalidUser);

            var response = await _requestManager.GetResponse(_endpointBuilder.BuildTokenEndpoint(), request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }
        [Test, Order(5)]
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

            var request = _requestManager.CreateGetRequest()
                .AddHeader("Authorization", ("Bearer " + _token));

            var response = await _requestManager.GetResponse(_endpointBuilder.BuildUserInfoEndpoint(), request);

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

            var request = _requestManager.CreatePostRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddJsonBody(job);

            var response = await _requestManager.GetResponse(_endpointBuilder.BuildCreateJobEndpoint(), request);

            dynamic tmp = JsonConvert.DeserializeObject(response.Content);
            _jobId = (long)tmp.jobId;

            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }
        [Test, Order(7)]
        public async Task CreateJob_InvalidTime_Failed()
        {
            var job = new
            {
                name = _bogus.Name.JobTitle(),
                description = _bogus.Name.JobDescriptor(),
                timeSchedule = _bogus.Date.Past()
            };

            var request = _requestManager.CreatePostRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddJsonBody(job);

            var response = await _requestManager.GetResponse(_endpointBuilder.BuildCreateJobEndpoint(), request);

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

            var request = _requestManager.CreateGetRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", _jobId);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildGetJobEndpoint(), request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, JsonConvert.DeserializeObject(response.Content).ToString()));
        }
        [Test, Order(9)]
        public async Task GetJob_NonexistentData_Failed()
        {
            var request = _requestManager.CreateGetRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", long.MaxValue);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildGetJobEndpoint(), request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }
        [Test, Order(10)]
        public async Task GetJob_InvalidData_Failed()
        {
            var request = _requestManager.CreateGetRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", -1);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildGetJobEndpoint(), request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }
        [Test, Order(11)]
        public async Task GetListJobs_CorrectData_Success()
        {
            var userId = new Repository().GetIdByUsername(_userCredentials.Username);
            var request = _requestManager.CreateGetRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("userId", userId);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildGetListJobsEndpoint(), request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
        [Test, Order(12)]
        public async Task CancelJob_CorrectData_Success()
        {
            var request = _requestManager.CreatePutRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", _jobId);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildCancelJobEndpoint(), request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
        [Test, Order(13)]
        public async Task CancelJob_InvalidData_Failed()
        {
            var request = _requestManager.CreatePutRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", -1);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildCancelJobEndpoint(), request);
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
            var request = _requestManager.CreatePutRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", _jobId)
                .AddJsonBody(job);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildCancelJobEndpoint(), request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
        [Test, Order(15)]
        public async Task UpdateJob_InvalidId_Failed()
        {
            var job = new
            {
                name = _bogus.Name.JobTitle(),
                description = _bogus.Name.JobDescriptor(),
                timeSchedule = _bogus.Date.FutureOffset()
            };

            var request = _requestManager.CreatePutRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("jobId", -1)
                .AddJsonBody(job);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildCancelJobEndpoint(), request);
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

            var request = _requestManager.CreatePostRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddHeader("Content-Type", "multipart/form-data")
                .AddQueryParameter("jobId", _jobId)
                .AddQueryParameter("width", _bogus.Random.Int(100, 2000))
                .AddQueryParameter("height", _bogus.Random.Int(100, 2000))
                .AddFile("httpRequest", @"C:\Users\i_moroz\Desktop\a.jpg");

            var response = await _requestManager.GetResponse(_endpointBuilder.BuildUploadImagesForJobEndpoint(), request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.IsTrue(IsValidJSONSchema(jsonSchema, response.Content));
        }
        [Test, Order(17)]
        public async Task UploadAvatar_CorrectData_Success()
        {
            var request = _requestManager.CreatePostRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddHeader("Content-Type", "multipart/form-data")
                .AddFile("file", @"C:\Users\i_moroz\Desktop\a.jpg");
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildUploadAvatarEndpoint(), request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);
        }
        [Test, Order(18)]
        public async Task UploadAvatar_EmptyImage_Failed()
        {
            var request = _requestManager.CreatePostRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddHeader("Content-Type", "multipart/form-data");

            var response = await _requestManager.GetResponse(_endpointBuilder.BuildUploadAvatarEndpoint(), request);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }
        [Test, Order(19)]
        public async Task DeleteUser_CorrectData_Success()
        {
            var userId = new Repository().GetIdByUsername(_userCredentials.Username);
            var request = _requestManager.CreateDeleteRequest()
                .AddHeader("Authorization", ("Bearer " + _token))
                .AddQueryParameter("id", userId);
            var response = await _requestManager.GetResponse(_endpointBuilder.BuildDeleteUserEndpoint(), request);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

    }
}