using Dapper;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MintyIssueTrackerTests
{
    public class Repository
    {
        private const string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=MintyIssueTrackerCore;Trusted_Connection=True;MultipleActiveResultSets=true";
        public T GetIdByKey<T>(string key, string value)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<T>($"SELECT * FROM {typeof(T).Name}s WHERE {key}='{value}'").SingleOrDefault();
            }
        }
    }
}
