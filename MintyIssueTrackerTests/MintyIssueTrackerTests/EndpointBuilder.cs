using MintyIssueTrackerTests.Logger;
using RestSharp;

namespace MintyIssueTrackerTests
{
    public class EndpointBuilder : RequestLogger
    {
        private const string BaseURL = "https://localhost:44333/";
        public RestClient BuildCancelJobEndpoint()
        {
            WriteToLog("Create endpoint: api/job/cancel");
            return new RestClient(BaseURL + "api/job/cancel");
        }

        public RestClient BuildCreateJobEndpoint()
        {
            WriteToLog("Create endpoint: api/job/create");
            return new RestClient(BaseURL + "api/job/create");
        }

        public RestClient BuildDeleteUserEndpoint()
        {
            WriteToLog("Create endpoint: api/user/remove");
            return new RestClient(BaseURL + "api/user/remove");
        }

        public RestClient BuildGetJobEndpoint()
        {
            WriteToLog("Create endpoint: api/job/get");
            return new RestClient(BaseURL + "api/job/get");
        }

        public RestClient BuildGetListJobsEndpoint()
        {
            WriteToLog("Create endpoint: api/job/list");
            return new RestClient(BaseURL + "api/job/list");
        }

        public RestClient BuildRegistrationEndpoint()
        {
            WriteToLog("Create endpoint: api/authentication/registration");
            return new RestClient(BaseURL + "api/authentication/registration");
        }

        public RestClient BuildTokenEndpoint()
        {
            WriteToLog("Create endpoint: api/authentication/token");
            return new RestClient(BaseURL + "api/authentication/token");
        }

        public RestClient BuildUploadAvatarEndpoint()
        {
            WriteToLog("Create endpoint: api/image/uploadavatar");
            return new RestClient(BaseURL + "api/image/uploadavatar");
        }

        public RestClient BuildUploadImagesForJobEndpoint()
        {
            WriteToLog("Create endpoint: api/job/uploadimages");
            return new RestClient(BaseURL + "api/job/uploadimages");
        }

        public RestClient BuildUserInfoEndpoint()
        {
            WriteToLog("Create endpoint: api/user/info");
            return new RestClient(BaseURL + "api/user/info");
        }
        public RestClient BuildUpdateJobEndpoint()
        {
            WriteToLog("Create endpoint: api/job/update");
            return new RestClient(BaseURL + "api/job/update");
        }
    }
}
