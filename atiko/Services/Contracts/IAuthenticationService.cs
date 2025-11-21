using atiko.Models;
using System.Threading.Tasks;

namespace atiko.Services.Contracts;

public interface IAuthenticationService
{
    User? CurrentUser { get; }
    Task<bool> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<User?> GetCurrentUserAsync();
}