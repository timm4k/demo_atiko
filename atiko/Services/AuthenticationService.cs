using atiko.Models;
using atiko.Services.Contracts;
using System;
using System.Threading.Tasks;

namespace atiko.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserDataService _userDataService;
    private User? _currentUser;

    public AuthenticationService(IUserDataService userDataService)
    {
        _userDataService = userDataService ?? throw new ArgumentNullException(nameof(userDataService));
    }

    public User? CurrentUser => _currentUser;

    public async Task<User?> GetCurrentUserAsync()
    {
        _currentUser = await _userDataService.GetCurrentUserAsync();
        return _currentUser;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var user = await _userDataService.LoginUserAsync(email, password);
        if (user != null)
        {
            _currentUser = user;
            return true;
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        await _userDataService.SetGuestModeAsync();
        _currentUser = null;
    }
}