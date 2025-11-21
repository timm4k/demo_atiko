using atiko.Models;
using atiko.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace atiko.Services;

public class UserDataService : IUserDataService
{
    private const string UsersFileName = "Users.json";
    private StorageFile? _usersFile;
    private UsersData _usersData;
    private bool _isInitialized;

    public UserDataService()
    {
        _usersData = new UsersData();
        _isInitialized = false;
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        try
        {
            var folder = ApplicationData.Current.LocalFolder;
            _usersFile = await folder.CreateFileAsync(UsersFileName, CreationCollisionOption.OpenIfExists);

            var json = await FileIO.ReadTextAsync(_usersFile);
            if (!string.IsNullOrEmpty(json))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                _usersData = JsonSerializer.Deserialize<UsersData>(json, options) ?? new UsersData();
            }
            else
            {
                _usersData = new UsersData();
                await SaveAsync();
            }
            _isInitialized = true;
        }
        catch (Exception)
        {
            _usersData = new UsersData();
            _isInitialized = true;
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_usersData, options);

            if (_usersFile == null)
            {
                var folder = ApplicationData.Current.LocalFolder;
                _usersFile = await folder.CreateFileAsync(UsersFileName, CreationCollisionOption.ReplaceExisting);
            }
            await FileIO.WriteTextAsync(_usersFile, json);
        }
        catch (Exception)
        {
        }
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        await InitializeAsync();
        return _usersData.CurrentUser;
    }

    public async Task RegisterUserAsync(User user, string password)
    {
        await InitializeAsync();

        if (string.IsNullOrEmpty(user.Email))
        {
            throw new ArgumentException("Email cannot be empty.");
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be empty.");
        }

        if (_usersData.Users.Any(u => u.Email == user.Email))
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        user.Id = user.Id ?? Guid.NewGuid().ToString();
        user.CreatedAt = DateTime.UtcNow.ToString("o");
        user.UpdatedAt = user.CreatedAt;
        user.Password = BCrypt.Net.BCrypt.HashPassword(password);
        user.ItemIds = new List<string>();
        user.UserTags = new List<string>();
        _usersData.Users.Add(user);
        _usersData.CurrentUser = user;
        await SaveAsync();
    }

    public async Task<User?> LoginUserAsync(string email, string password)
    {
        await InitializeAsync();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var user = _usersData.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(user.Password))
        {
            return null;
        }

        try
        {
            if (BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                user.LastLogin = DateTime.UtcNow.ToString("o");
                _usersData.CurrentUser = user;
                await SaveAsync();
                return user;
            }
            else
            {
                return null;
            }
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return null;
        }
    }

    public async Task UpdateUserAsync(User updatedUser)
    {
        await InitializeAsync();
        var user = _usersData.Users.FirstOrDefault(u => u.Email == updatedUser.Email);
        if (user != null)
        {
            user.Name = updatedUser.Name;
            user.AvatarUri = updatedUser.AvatarUri;
            user.Description = updatedUser.Description;
            user.Gender = updatedUser.Gender;
            user.Birthday = updatedUser.Birthday;
            user.ItemIds = updatedUser.ItemIds ?? new List<string>();
            user.UserTags = updatedUser.UserTags ?? new List<string>();
            user.UpdatedAt = DateTime.UtcNow.ToString("o");
            if (_usersData.CurrentUser?.Email == user.Email)
            {
                _usersData.CurrentUser = user;
            }
            await SaveAsync();
        }
    }

    public async Task<UserStatistics> GetUserStatisticsAsync()
    {
        await InitializeAsync();

        var newestUserDate = _usersData.Users
            .Where(u => !string.IsNullOrEmpty(u.CreatedAt))
            .Select(u => TryParseDateTime(u.CreatedAt))
            .Where(d => d != null)
            .Select(d => d!.Value)
            .DefaultIfEmpty(DateTime.MinValue)
            .Max();

        var oldestUserDate = _usersData.Users
            .Where(u => !string.IsNullOrEmpty(u.CreatedAt))
            .Select(u => TryParseDateTime(u.CreatedAt))
            .Where(d => d != null)
            .Select(d => d!.Value)
            .DefaultIfEmpty(DateTime.MinValue)
            .Min();

        var stats = new UserStatistics
        {
            TotalUsers = _usersData.Users.Count,
            ActiveUsers = _usersData.Users.Count(u => u.LastLogin != null),
            AverageNotifications = _usersData.Users.Any() ? _usersData.Users.Average(u => u.NotificationCount ?? 0) : 0,
            NewestUserDate = newestUserDate,
            OldestUserDate = oldestUserDate
        };
        return stats;
    }

    private DateTime? TryParseDateTime(string? s)
    {
        if (!string.IsNullOrWhiteSpace(s) && DateTime.TryParse(s, out var dt))
            return dt;
        return null;
    }

    public async Task ResetPasswordAsync(string email, string newPassword)
    {
        await InitializeAsync();
        var user = _usersData.Users.FirstOrDefault(u => u.Email == email);
        if (user != null)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow.ToString("o");
            await SaveAsync();
        }
        else
        {
            throw new Exception("User not found");
        }
    }

    public async Task AddNotificationAsync(string email)
    {
        await InitializeAsync();
        var user = _usersData.Users.FirstOrDefault(u => u.Email == email);
        if (user != null)
        {
            user.NotificationCount = (user.NotificationCount ?? 0) + 1;
            user.UpdatedAt = DateTime.UtcNow.ToString("o");
            await SaveAsync();
        }
    }

    public async Task SetGuestModeAsync()
    {
        await InitializeAsync();
        _usersData.CurrentUser = null;
        await SaveAsync();
    }

    public async Task AddItemIdAsync(string email, string itemId)
    {
        await InitializeAsync();
        var user = _usersData.Users.FirstOrDefault(u => u.Email == email);
        if (user != null && !user.ItemIds.Contains(itemId))
        {
            user.ItemIds.Add(itemId);
            user.UpdatedAt = DateTime.UtcNow.ToString("o");
            await SaveAsync();
        }
    }

    public async Task AddUserTagAsync(string email, string tag)
    {
        await InitializeAsync();
        var user = _usersData.Users.FirstOrDefault(u => u.Email == email);
        if (user != null && !user.UserTags.Contains(tag))
        {
            user.UserTags.Add(tag);
            user.UpdatedAt = DateTime.UtcNow.ToString("o");
            await SaveAsync();
        }
    }
}