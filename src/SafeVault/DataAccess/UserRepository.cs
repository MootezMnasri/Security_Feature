using System;
using System.Data;
using MySql.Data.MySqlClient;
using SafeVault.Security;

namespace SafeVault.DataAccess
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }

    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Retrieves a user by username using parameterized queries to prevent SQL injection
        public User GetUserByUsername(string username)
        {
            var safeUsername = SafeVault.Security.InputValidator.SanitizeUsername(username);

            using (var conn = new MySqlConnection(_connectionString))
            {
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, Role
                    FROM Users
                    WHERE Username = @Username
                ";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Username", MySqlDbType.VarChar).Value = (object)safeUsername ?? DBNull.Value;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                PasswordHash = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Role = reader.IsDBNull(4) ? "User" : reader.GetString(4)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
