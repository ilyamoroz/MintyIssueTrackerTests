using Bogus;
using MintyIssueTrackerTests.Model;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace MintyIssueTrackerTests.Directors
{
    public static class AuthenticationDirector
    {
        private static EndpointBuilder _endpointBuilder = new EndpointBuilder();
        private static Faker _bogus = new Faker();

        public static async Task<UserModel> CreateCredentials()
        {
            var body = new CreateUserModel()
            {
                username = _bogus.Random.String2(minLength: 5, maxLength: 16),
                password = _bogus.Random.String2(minLength: 5, maxLength: 16),
                firstname = _bogus.Name.FirstName(),
                lastname = _bogus.Name.LastName()
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
                    Username = body.username,
                    Password = body.password
                };
            }
            else
            {
                throw new ArgumentNullException("User Credentials");
            }
        }

        public static async Task<string> GetToken(UserModel credentials)
        {
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
                throw new ArgumentNullException("Access token");
            }
        }
    }
}
