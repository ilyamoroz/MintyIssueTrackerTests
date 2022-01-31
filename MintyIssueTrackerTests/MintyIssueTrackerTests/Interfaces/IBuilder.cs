using RestSharp;

namespace MintyIssueTrackerTests.Interfaces
{
    public interface IBuilder
    {
        RestClient BuildRegistrationEndpoint();
        RestClient BuildTokenEndpoint();
        RestClient BuildUserInfoEndpoint();
        RestClient BuildCreateJobEndpoint();
        RestClient BuildGetJobEndpoint();
        RestClient BuildGetListJobsEndpoint();
        RestClient BuildCancelJobEndpoint();
        RestClient BuildUploadImagesForJobEndpoint();
        RestClient BuildUploadAvatarEndpoint();
        RestClient BuildDeleteUserEndpoint();
    }
}
