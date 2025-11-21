using atiko.Common;
using atiko.Models;
using atiko.Services.Contracts;
using atiko.Views;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace atiko.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IUserDataService _userDataService;
    private readonly INavigationService _navigationService;
    private string? _selectedLanguage;
    private ObservableCollection<LanguageOption> _availableLanguages;
    private string? _saveMessage;
    private string? _errorMessage;
    private bool _isDarkTheme;
    private UserStatistics? _userStatistics;
    private User? _currentUser;
    private string? _avatarUri;
    private bool _isGuestMode;

    private RelayCommand _saveSettingsCommand;
    private RelayCommand _uploadAvatarCommand;
    private RelayCommand _goToLoginCommand;
    private RelayCommand _goToSignUpCommand;

    public SettingsViewModel(IUserDataService userDataService, INavigationService navigationService)
    {
        _userDataService = userDataService ?? throw new ArgumentNullException(nameof(userDataService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _selectedLanguage = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride ?? "en-US";
        _availableLanguages = new ObservableCollection<LanguageOption>
        {
            new LanguageOption { DisplayName = "English", LanguageCode = "en-US" },
            new LanguageOption { DisplayName = "Ukrainian", LanguageCode = "uk-UA" }
        };
        _isDarkTheme = (ApplicationData.Current.LocalSettings.Values["Theme"] as string) == ElementTheme.Dark.ToString();
        _saveSettingsCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
        _uploadAvatarCommand = new RelayCommand(async () => await ExecuteUploadAvatarAsync());
        _goToLoginCommand = new RelayCommand(() => NavigateToAuthPage(AuthViewModel.AuthMode.Login));
        _goToSignUpCommand = new RelayCommand(() => NavigateToAuthPage(AuthViewModel.AuthMode.SignUp));
        _ = LoadDataAsync();
    }

    public void Initialize(Window window)
    {
        Debug.WriteLine("SettingsViewModel.Initialize: Starting data load");
        _ = LoadDataAsync();
    }

    public async Task OnNavigatedTo()
    {
        Debug.WriteLine("SettingsViewModel.OnNavigatedTo: Refreshing user data after navigation");
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            Debug.WriteLine("SettingsViewModel.LoadDataAsync: Loading user data");
            CurrentUser = await _userDataService.GetCurrentUserAsync();
            UserStatistics = await _userDataService.GetUserStatisticsAsync();
            IsGuestMode = CurrentUser == null;
            AvatarUri = CurrentUser?.ImageUri;
            Debug.WriteLine($"SettingsViewModel.LoadDataAsync: CurrentUser is {(CurrentUser == null ? "null" : CurrentUser.Email)}, IsGuestMode: {IsGuestMode}, AvatarUri: {AvatarUri}");
            OnPropertyChanged(nameof(IsGuestMode));
            OnPropertyChanged(nameof(AvatarUri));
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(UserStatistics));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SettingsViewModel.LoadDataAsync: Error loading data: {ex.Message}");
            ErrorMessage = $"Failed to load settings: {ex.Message}";
        }
    }

    public async Task LogoutAsync()
    {
        if (IsGuestMode)
        {
            SaveMessage = "You already logged out";
            Debug.WriteLine("Logout attempted in guest mode: No action taken");
            return;
        }

        try
        {
            Debug.WriteLine("SettingsViewModel.LogoutAsync: Logging out user");
            await _userDataService.SetGuestModeAsync();
            SaveMessage = "Logged out successfully";
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Logout failed: {ex.Message}";
            SaveMessage = null;
            Debug.WriteLine($"Logout error: {ex}");
        }
    }

    public ObservableCollection<LanguageOption> AvailableLanguages
    {
        get => _availableLanguages;
        set => SetProperty(ref _availableLanguages, value);
    }

    public string? SelectedLanguage
    {
        get => _selectedLanguage;
        set => SetProperty(ref _selectedLanguage, value);
    }

    public string? SaveMessage
    {
        get => _saveMessage;
        set
        {
            SetProperty(ref _saveMessage, value);
            OnPropertyChanged(nameof(SaveMessageVisibility));
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            SetProperty(ref _errorMessage, value);
            OnPropertyChanged(nameof(ErrorMessageVisibility));
        }
    }

    public bool SaveMessageVisibility => !string.IsNullOrEmpty(SaveMessage);
    public bool ErrorMessageVisibility => !string.IsNullOrEmpty(ErrorMessage);

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set => SetProperty(ref _isDarkTheme, value);
    }

    public UserStatistics? UserStatistics
    {
        get => _userStatistics;
        set => SetProperty(ref _userStatistics, value);
    }

    public User? CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value);
    }

    public string? AvatarUri
    {
        get => _avatarUri;
        set => SetProperty(ref _avatarUri, value);
    }

    public bool IsGuestMode
    {
        get => _isGuestMode;
        set
        {
            SetProperty(ref _isGuestMode, value);
            Debug.WriteLine($"SettingsViewModel.IsGuestMode: Set to {value}");
            OnPropertyChanged(nameof(IsGuestMode));
        }
    }

    public ICommand SaveSettingsCommand => _saveSettingsCommand;
    public ICommand UploadAvatarCommand => _uploadAvatarCommand;
    public ICommand GoToLoginCommand => _goToLoginCommand;
    public ICommand GoToSignUpCommand => _goToSignUpCommand;

    public async Task SaveAsync()
    {
        try
        {
            Debug.WriteLine("SettingsViewModel.SaveAsync: Saving settings");
            var localSettings = ApplicationData.Current.LocalSettings;
            var currentTheme = localSettings.Values["Theme"] as string;
            if (IsDarkTheme != (currentTheme == ElementTheme.Dark.ToString()))
            {
                var newTheme = IsDarkTheme ? ElementTheme.Dark : ElementTheme.Light;
                localSettings.Values["Theme"] = newTheme.ToString();
                if (Application.Current is App app && app.MainWindow?.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = newTheme;
                }
            }

            if (_selectedLanguage != null && _selectedLanguage != Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride)
            {
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = _selectedLanguage;
                localSettings.Values["Language"] = _selectedLanguage;
            }

            if (_currentUser != null && !string.IsNullOrEmpty(_avatarUri))
            {
                _currentUser.ImageUri = _avatarUri;
                await _userDataService.UpdateUserAsync(_currentUser);
            }

            SaveMessage = "Settings saved successfully";
            ErrorMessage = null;
            Debug.WriteLine("SettingsViewModel.SaveAsync: Settings saved successfully");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving settings: {ex.Message}";
            SaveMessage = null;
            Debug.WriteLine($"SettingsViewModel.SaveAsync: Save error: {ex}");
        }
    }

    private bool CanSave() => true;

    private async Task ExecuteUploadAvatarAsync()
    {
        try
        {
            Debug.WriteLine("SettingsViewModel.ExecuteUploadAvatarAsync: Starting avatar upload");
            ErrorMessage = null;
            SaveMessage = null;

            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var mainWindow = ((App)Application.Current).MainWindow;
            if (mainWindow == null)
            {
                ErrorMessage = "Unable to open file picker: Main window not initialized";
                Debug.WriteLine("SettingsViewModel.ExecuteUploadAvatarAsync: Avatar upload failed: Main window not initialized");
                return;
            }

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                SaveMessage = "No file selected";
                Debug.WriteLine("SettingsViewModel.ExecuteUploadAvatarAsync: Avatar upload cancelled: No file selected");
                return;
            }

            string extension = Path.GetExtension(file.Name).ToLower();
            if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
            {
                ErrorMessage = "Only PNG or JPG files are allowed";
                Debug.WriteLine($"SettingsViewModel.ExecuteUploadAvatarAsync: Avatar upload failed: Invalid file type {extension}");
                return;
            }

            AvatarUri = file.Path;
            Debug.WriteLine($"SettingsViewModel.ExecuteUploadAvatarAsync: Avatar selected: {AvatarUri}");

            if (_currentUser != null)
            {
                await SaveAsync();
                SaveMessage = "Avatar uploaded successfully";
            }
            else
            {
                SaveMessage = "Avatar selected, but not saved (you are in guest mode)";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to upload avatar: {ex.Message}";
            SaveMessage = null;
            Debug.WriteLine($"SettingsViewModel.ExecuteUploadAvatarAsync: Avatar upload error: {ex}");
        }
    }

    private void NavigateToAuthPage(AuthViewModel.AuthMode mode)
    {
        Debug.WriteLine($"SettingsViewModel.NavigateToAuthPage: Navigating to AuthPage with mode {mode}");
        _navigationService.NavigateTo(typeof(AuthPage), mode);
    }
}