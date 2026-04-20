using Microsoft.AspNetCore.Identity;
using OidcServer.Models;

namespace OidcServer.Repositories
{
    public interface IUserRepository
    {
        User? GetUserByUsername(string username);
        bool ValidateUserCredentials( string username ,string password);
    }
}
