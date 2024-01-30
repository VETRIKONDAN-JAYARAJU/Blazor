using System.Data;
using MySqlConnector;
using Microsoft.Extensions.Configuration;

namespace DataServices
{
    public class DBConnection
    {
        private readonly IConfiguration _configuration;

        public DBConnection(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IDbConnection Create()
        {
            return new MySqlConnection(_configuration.GetConnectionString("MariaDBConnection"));
        }
    }
}
