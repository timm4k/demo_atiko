using atiko.Models;
using System.Threading.Tasks;

namespace atiko.Services.Contracts;

public interface IUserDataService
{
    Task<User?> GetCurrentUserAsync();
    Task RegisterUserAsync(User user, string password);
    Task<User?> LoginUserAsync(string email, string password);
    Task UpdateUserAsync(User updatedUser);
    Task<UserStatistics> GetUserStatisticsAsync();
    Task ResetPasswordAsync(string email, string newPassword);
    Task AddNotificationAsync(string email);
    Task SetGuestModeAsync();
    Task AddItemIdAsync(string email, string itemId);
    Task AddUserTagAsync(string email, string tag);
}