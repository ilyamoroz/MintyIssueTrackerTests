using RestSharp;
using System.Threading.Tasks;

namespace MintyIssueTrackerTests
{
    /// <summary>
    /// Request manager
    /// </summary>
    public class RequestManager
    {
        /// <summary>
        /// Create request with post method
        /// </summary>
        public RestRequest CreatePostRequest()
        {
            var restRequest = new RestRequest();
            restRequest.Method = Method.Post;
            restRequest.AddHeader("Accept", "application/json");
            return restRequest;
        }

        /// <summary>
        /// Create request with put method
        /// </summary>
        public RestRequest CreatePutRequest()
        {
            var restRequest = new RestRequest();
            restRequest.Method = Method.Put;
            restRequest.AddHeader("Accept", "application/json");
            return restRequest;
        }

        /// <summary>
        /// Create request with get method
        /// </summary>
        public RestRequest CreateGetRequest()
        {
            var restRequest = new RestRequest();
            restRequest.Method = Method.Get;
            restRequest.AddHeader("Accept", "application/json");
            return restRequest;
        }

        /// <summary>
        /// Create request with delete method
        /// </summary>
        public RestRequest CreateDeleteRequest()
        {
            var restRequest = new RestRequest();
            restRequest.Method = Method.Delete;
            restRequest.AddHeader("Accept", "application/json");
            return restRequest;
        }

        /// <summary>
        /// Execute request
        /// </summary>
        public async Task<RestResponse> GetResponse(RestClient client, RestRequest request)
        {
            return await client.ExecuteAsync(request);
        }
    }
}
