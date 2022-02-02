using System.Threading.Tasks;
using RestSharp;

namespace MintyIssueTrackerTests
{
    public static class RequestFactory
    {
        public static RequestManager RequestManager { get; set; } = new RequestManager();
        public static RestRequest SetApiEndpoint(this RestRequest request, RestClient client)
        {
            RequestManager.SetClient(client);
            return request;
        }
        public static RestRequest SetBody(this RestRequest request,  object body)
        {
            request.AddBody(body);
            return request;
        }
        public static RestRequest SetToken(this RestRequest request, string token)
        {
            return request.AddHeader("Authorization", ("Bearer " + token));
        }
        public static RestRequest SetFormData(this RestRequest request)
        {
            return request.AddHeader("Content-Type", "multipart/form-data");
        }
        public static async Task<RestResponse> SendRequest(this RestRequest request)
        {
            return await RequestManager.GetResponse(request);
        }
    }
}
