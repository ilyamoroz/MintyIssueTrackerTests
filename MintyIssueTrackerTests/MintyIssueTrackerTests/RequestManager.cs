using RestSharp;
using System.Threading.Tasks;

namespace MintyIssueTrackerTests
{
    public class RequestManager
    {
        private RestClient _client;

        public void SetClient(RestClient client)
        {
            _client = client;
        }

        public RestRequest CreatePostRequest()
        {
            var restRequest = new RestRequest();
            restRequest.Method = Method.Post;
            restRequest.AddHeader("Accept", "application/json");
            return restRequest;
        }
        public RestRequest CreatePutRequest()
        {
            var restRequest = new RestRequest();
            restRequest.Method = Method.Put;
            restRequest.AddHeader("Accept", "application/json");
            return restRequest;
        }

        public RestRequest CreateGetRequest()
        {
            var restRequest = new RestRequest();
            restRequest.Method = Method.Get;
            restRequest.AddHeader("Accept", "application/json");
            return restRequest;
        }

        public RestRequest CreateDeleteRequest()
        {
            var restRequest = new RestRequest();
            restRequest.Method = Method.Delete;
            restRequest.AddHeader("Accept", "application/json");
            return restRequest;
        }

        public async Task<RestResponse> GetResponse(RestRequest request)
        {
            return await _client.ExecuteAsync(request);
        }
    }
}
