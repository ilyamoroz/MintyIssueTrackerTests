using RestSharp;

namespace MintyIssueTrackerTests
{
    public class EndpointBuilder
    {
        private const string BaseURL = "https://localhost:44333/";
        public RestClient BuildCancelJobEndpoint()
        {
            return new RestClient(BaseURL + "api/job/cancel");
        }

        public RestClient BuildCreateJobEndpoint()
        {
            return new RestClient(BaseURL + "api/job/create");
        }

        public RestClient BuildDeleteUserEndpoint()
        {
            return new RestClient(BaseURL + "api/user/remove");
        }

        public RestClient BuildGetJobEndpoint()
        {
            return new RestClient(BaseURL + "api/job/get");
        }

        public RestClient BuildGetListJobsEndpoint()
        {
            return new RestClient(BaseURL + "api/job/list");
        }

        public RestClient BuildRegistrationEndpoint()
        {
            return new RestClient(BaseURL + "api/authentication/registration");
        }

        public RestClient BuildTokenEndpoint()
        {
            return new RestClient(BaseURL + "api/authentication/token");
        }

        public RestClient BuildUploadAvatarEndpoint()
        {
            return new RestClient(BaseURL + "api/image/uploadavatar");
        }

        public RestClient BuildUploadImagesForJobEndpoint()
        {
            return new RestClient(BaseURL + "api/job/uploadimages");
        }

        public RestClient BuildUserInfoEndpoint()
        {
            return new RestClient(BaseURL + "api/user/info");
        }
    }
}
