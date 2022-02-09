using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.IO;

namespace MintyIssueTrackerTests
{
    public static class JSONHelper
    {
        public const string CreateJobResponseSchemaPath = @"JsonSchemas\CreateJobResponseSchema.json";
        public const string GetJobResponseSchemaPath = @"JsonSchemas\GetJobResponseSchema.json";
        public const string UploadImageResponseSchemaPath = @"JsonSchemas\UploadImageResponseSchema.json";
        public const string GetUserResponseSchemaPath = @"JsonSchemas\GetUserResponseSchema.json";
        public const string RegisterUserResponseSchemaPath = @"JsonSchemas\RegisterUserResponseSchema.json";
        public const string GetAccessTokenResponseSchemaPath = @"JsonSchemas\GetAccessTokenResponseSchema.json";
        /// <summary>
        /// Validate json string with json schema
        /// </summary>
        public static bool IsValidJSONSchema(string jsonSchemaPath, string json)
        {
            var directory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

            using (StreamReader r = new StreamReader(Path.Combine(directory, jsonSchemaPath)))
            {
                var jsonSchema = r.ReadToEnd();
                JsonSchema schema = JsonSchema.Parse(jsonSchema);
                var data = JObject.Parse(json);
                return data.IsValid(schema);
            }
        }

    }
}
