using Dapper;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MintyIssueTrackerTests.Tests;

namespace MintyIssueTrackerTests
{
    public static class Repository
    {
        /// <summary>
        /// Returns value from database
        /// </summary>
        public static T GetByKey<T>(string key, string value)
        {
            var _connectionString = new ConfigurationBuilder().AddUserSecrets<UserTest>()
                .Build().GetSection("ConnectionString").Value;

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<T>($"SELECT * FROM {typeof(T).Name}s WHERE {key}='{value}'").SingleOrDefault();
            }
        }
    }
}
