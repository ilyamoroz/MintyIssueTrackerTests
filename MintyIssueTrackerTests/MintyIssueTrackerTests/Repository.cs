using Dapper;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MintyIssueTrackerTests
{
    public class Repository
    {
        private const string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=MintyIssueTrackerCore;Trusted_Connection=True;MultipleActiveResultSets=true";
        public long GetIdByUsername(string username)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<long>($"SELECT Id FROM Users WHERE Username='{username}'").SingleOrDefault();
            }
        }
    }
}
