using Bogus;
using MintyIssueTrackerTests.Model;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using System;
using MintyIssueTrackerTests.Logger;
using Newtonsoft.Json;

namespace MintyIssueTrackerTests.Directors
{
    public static class AuthenticationDirector
    {
        private static EndpointBuilder _endpointBuilder = new EndpointBuilder();
        private static RequestLogger _logger = new RequestLogger();
        private static Faker _bogus = new Faker();

        /// <summary>
        /// Create user for test
        /// </summary>
        public static async Task<UserModel> CreateCredentials()
        {
            _logger.WriteToLog("Create credentials for test");
            var body = new CreateUserModel()
            {
                Username = _bogus.Random.String2(minLength: 5, maxLength: 16),
                Password = _bogus.Random.String2(minLength: 7, maxLength: 16),
                Firstname = _bogus.Name.FirstName(),
                Lastname = _bogus.Name.LastName()
            };

            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildRegistrationEndpoint())
                .SetBody(body)
                .AddQueryParameter("roleName", "admin")
                .SendRequest();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new UserModel()
                {
                    Username = body.Username,
                    Password = body.Password
                };
            }
            else
            {
                throw new ArgumentNullException($"User Credentials. Response content {response.Content}, response code {response.StatusCode}");
            }
        }

        /// <summary>
        /// Returns token for tests
        /// </summary>
        public static async Task<string> GetToken(UserModel credentials)
        {
            _logger.WriteToLog("Get token for test");
            var response = await RequestFactory
                .RequestManager
                .CreatePostRequest()
                .SetApiEndpoint(_endpointBuilder.BuildTokenEndpoint())
                .SetBody(credentials)
                .SendRequest();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<TokenModel>(response.Content).access_token;
            }
            else
            {
                throw new ArgumentNullException($"Access token. Response content {response.Content}, response code {response.StatusCode}"); ; ; ;
            }
        }
    }
}
