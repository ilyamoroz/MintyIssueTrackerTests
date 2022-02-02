using Bogus;
using MintyIssueTrackerTests.ExtentionMethods;
using MintyIssueTrackerTests.Model;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace MintyIssueTrackerTests.Directors
{
    public static class JobDirector
    {
        private static Faker _bogus = new Faker();
        private static EndpointBuilder _endpointBuilder = new EndpointBuilder();
        
        public static async Task<long> GetJobId(string token)
        {
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
                .SetToken(token)
                .SetBody(job)
                .SendRequest();

            return JsonConvert.DeserializeObject<CreateJobResponseModel>(response.Content).jobId;
            
        }
    }
}
