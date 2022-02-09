using System;
using System.Net;
using Bogus;
using MintyIssueTrackerTests.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MintyIssueTrackerTests.Logger;

namespace MintyIssueTrackerTests.Directors
{
    public static class JobDirector
    {
        private static Faker _bogus = new Faker();
        private static RequestLogger _logger = new RequestLogger();
        private static EndpointBuilder _endpointBuilder = new EndpointBuilder();

        /// <summary>
        /// Returns job id for test
        /// </summary>
        public static async Task<long> GetJobId(string token)
        {
            _logger.WriteToLog("Create job for test");
            var job = new JobModel
            {
                Name = _bogus.Random.String2(minLength: 6, maxLength: 64),
                Description = _bogus.Random.String2(minLength: 6, maxLength: 256),
                TimeSchedule = _bogus.Date.Future()
            };

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildCreateJobEndpoint())
                .SetToken(token)
                .SetBody(job)
                .SendRequest();

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return JsonConvert.DeserializeObject<CreateJobResponseModel>(response.Content).jobId;
            }
            else
            {
                throw new ArgumentNullException($"Job. Response content {response.Content}, response code {response.StatusCode}");
            }
        }
    }
}
