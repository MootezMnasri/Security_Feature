using SafeVault.DataAccess;

namespace SafeVault.DataAccess
{
    // Simple contract for user lookups used by authentication/authorization
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
    }
}
