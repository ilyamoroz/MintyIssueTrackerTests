using System.Threading.Tasks;
using MintyIssueTrackerTests.Logger;
using Newtonsoft.Json;
using RestSharp;

namespace MintyIssueTrackerTests
{
    public static class RequestFactory
    {
        public static RequestManager RequestManager { get; set; } = new RequestManager();
        private static RequestLogger _logger = new RequestLogger();
        public static RestRequest SetApiEndpoint(this RestRequest request, RestClient client)
        {
            RequestManager.SetClient(client);
            return request;
        }
        public static RestRequest SetBody(this RestRequest request,  object body)
        {
            _logger.WriteToLog("Add body: " + JsonConvert.SerializeObject(body));
            request.AddBody(body);
            return request;
        }
        public static RestRequest SetToken(this RestRequest request, string token)
        {
            _logger.WriteToLog("Add token");
            return request.AddHeader("Authorization", ("Bearer " + token));
        }
        public static RestRequest SetFormData(this RestRequest request)
        {
            _logger.WriteToLog("Add Form-Data");
            return request.AddHeader("Content-Type", "multipart/form-data");
        }
        public static async Task<RestResponse> SendRequest(this RestRequest request)
        {
            _logger.WriteToLog("Send request");
            return await RequestManager.GetResponse(request);
        }
    }
}
