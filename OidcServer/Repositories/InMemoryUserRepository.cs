using OidcServer.Models;

namespace OidcServer.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private  List<User> _users = new()
        {
            new() { Username = "bob", Password = "bobsecurtiry" },
            new() { Username = "alice", Password = "alicesecurity" },
            new () { Username = "tin", Password = "tindeptrai" },
        };
        public User? GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username.Equals(username));
        }

        public bool ValidateUserCredentials(string username, string password)
        {
           return GetUserByUsername(username) != null && GetUserByUsername(username)?.Password == password;
        }
    }
}
