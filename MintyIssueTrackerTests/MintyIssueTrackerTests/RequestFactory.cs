using System.Threading.Tasks;
using MintyIssueTrackerTests.Logger;
using Newtonsoft.Json;
using RestSharp;

namespace MintyIssueTrackerTests
{
    /// <summary>
    /// Request Factory
    /// </summary>
    public static class RequestFactory
    {
        public static RequestManager RequestManager { get; set; } = new RequestManager();

        private static RequestLogger _logger = new RequestLogger();
        private static RestClient _client;

        public static RestRequest SetApiEndpoint(this RestRequest request, RestClient endpointRequest)
        {
            _client = endpointRequest;
            return request;
        }

        /// <summary>
        /// Set body for request
        /// </summary>
        public static RestRequest SetBody(this RestRequest request,  object body)
        {
            _logger.WriteToLog("Add body: " + JsonConvert.SerializeObject(body));
            request.AddBody(body);
            return request;
        }

        /// <summary>
        /// Set token for request
        /// </summary>
        public static RestRequest SetToken(this RestRequest request, string token)
        {
            _logger.WriteToLog("Add token");
            return request.AddHeader("Authorization", ("Bearer " + token));
        }

        /// <summary>
        /// Set forn-data for request
        /// </summary>
        public static RestRequest SetFormData(this RestRequest request)
        {
            _logger.WriteToLog("Add Form-Data");
            return request.AddHeader("Content-Type", "multipart/form-data");
        }

        /// <summary>
        /// Send request
        /// </summary>
        public static async Task<RestResponse> SendRequest(this RestRequest request)
        {
            _logger.WriteToLog("Send request");
            return await RequestManager.GetResponse(_client, request);
        }
    }
}
