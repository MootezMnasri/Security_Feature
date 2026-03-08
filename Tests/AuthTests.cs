using System;
using NUnit.Framework;
using SafeVault.DataAccess;
using SafeVault.Security;
using System.Collections.Generic;

namespace Tests
{
    // Simple in-memory fake repository for unit tests
    public class FakeUserRepository : IUserRepository
    {
        private readonly Dictionary<string, User> _store = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);
        public void Add(User user) => _store[user.Username] = user;
        public User GetUserByUsername(string username) => _store.TryGetValue(username, out var u) ? u : null;
    }

    [TestFixture]
    public class AuthTests
    {
        [Test]
        public void Authenticate_ValidCredentials_ReturnsUser()
        {
            var repo = new FakeUserRepository();
            var hash = BCrypt.Net.BCrypt.HashPassword("correct-password");
            var user = new User { UserID = 1, Username = "jane", Email = "jane@example.com", PasswordHash = hash, Role = "Admin" };
            repo.Add(user);

            var auth = new AuthService(repo);
            var result = auth.Authenticate("jane", "correct-password");
            Assert.IsNotNull(result);
            Assert.AreEqual("jane", result.Username);
        }

        [Test]
        public void Authenticate_InvalidPassword_ReturnsNull()
        {
            var repo = new FakeUserRepository();
            var hash = BCrypt.Net.BCrypt.HashPassword("secret");
            var user = new User { UserID = 2, Username = "bob", Email = "bob@example.com", PasswordHash = hash, Role = "User" };
            repo.Add(user);

            var auth = new AuthService(repo);
            var result = auth.Authenticate("bob", "wrong-password");
            Assert.IsNull(result);
        }

        [Test]
        public void Authorization_AdminAccess_TassesRole()
        {
            var admin = new User { UserID = 3, Username = "admin", Email = "admin@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"), Role = "Admin" };
            var user = new User { UserID = 4, Username = "user", Email = "user@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"), Role = "User" };
            var authz = new AuthorizationService();
            Assert.IsTrue(authz.IsAdmin(admin));
            Assert.IsFalse(authz.IsAdmin(user));
        }
    }
}
