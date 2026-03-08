using System;
using SafeVault.DataAccess;
using SafeVault.Security;
using BCrypt.Net;

namespace SafeVault.Security
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        // DI-friendly ctor
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        // Convenience ctor for quick setup (requires a concrete UserRepository)
        public AuthService(string connectionString) : this(new UserRepository(connectionString)) { }

        // Authenticate by username and password using bcrypt hash verification
        public User Authenticate(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash)) return null;
            // Verify password against stored hash
            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        // Simple role-check helper
        public bool IsInRole(User user, string role) => user != null && string.Equals(user.Role, role, StringComparison.OrdinalIgnoreCase);
        
        // Optional: hash a plain password (useful for seeding test users)
        public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    }
}
