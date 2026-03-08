using System;
using SafeVault.DataAccess;

namespace SafeVault.Security
{
    public class AuthorizationService
    {
        // Return true if user is in the specified role
        public bool CanAccess(User user, string requiredRole) =>
            user != null && string.Equals(user.Role, requiredRole, StringComparison.OrdinalIgnoreCase);

        // Convenience helpers for common roles
        public bool IsAdmin(User user) => CanAccess(user, "Admin");
        public bool IsUser(User user) => CanAccess(user, "User");
    }
}
