using atiko.Common;
using atiko.Models;
using atiko.Services.Contracts;
using atiko.Views;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace atiko.ViewModels;

public partial class AuthViewModel : ViewModelBase
{
    public enum AuthMode { Login, SignUp, Change, Guest }

    private readonly INavigationService _navigationService;
    private readonly IUserDataService _userDataService;

    private AuthMode _authMode = AuthMode.Login;
    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _oldPassword = string.Empty;
    private string _newPassword = string.Empty;
    private string _gender = string.Empty;
    private DateTimeOffset? _birthday = null;
    private string? _avatarUri;
    private string _errorMessage = string.Empty;
    private DataTemplate _currentTemplate;
    private User? _currentUser;

    private RelayCommand _loginCommand;
    private RelayCommand _registerCommand;
    private RelayCommand _changePasswordCommand;
    private RelayCommand _uploadAvatarCommand;
    private RelayCommand _goLoginCommand;
    private RelayCommand _goSignUpCommand;
    private RelayCommand _goChangePasswordCommand;
    private RelayCommand _goGuestModeCommand;

    public AuthViewModel(INavigationService navigationService, IUserDataService userDataService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _userDataService = userDataService ?? throw new ArgumentNullException(nameof(userDataService));

        _uploadAvatarCommand = new RelayCommand(ExecuteUploadAvatar);
        _loginCommand = new RelayCommand(ExecuteLogin, () => CanLogin);
        _registerCommand = new RelayCommand(ExecuteRegister, () => CanRegister);
        _changePasswordCommand = new RelayCommand(ExecuteChangePassword, () => CanChangePassword);
        _goLoginCommand = new RelayCommand(() => Mode = AuthMode.Login);
        _goSignUpCommand = new RelayCommand(() => Mode = AuthMode.SignUp);
        _goChangePasswordCommand = new RelayCommand(() => Mode = AuthMode.Change);
        _goGuestModeCommand = new RelayCommand(() => Mode = AuthMode.Guest);

        _currentTemplate = (DataTemplate)Application.Current.Resources["LoginTemplate"];
        _ = LoadCurrentUserAsync();
        UpdateCurrentTemplate();
    }

    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            if (SetProperty(ref _currentUser, value))
            {
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }
    }

    public bool IsLoggedIn => CurrentUser != null;

    public AuthMode Mode
    {
        get => _authMode;
        set
        {
            if (SetProperty(ref _authMode, value))
            {
                OnPropertyChanged(nameof(IsLoginMode));
                OnPropertyChanged(nameof(IsSignUpMode));
                OnPropertyChanged(nameof(IsChangePasswordMode));
                OnPropertyChanged(nameof(IsGuestMode));
                OnPropertyChanged(nameof(Title));
                ClearForm();
                UpdateCurrentTemplate();
            }
        }
    }

    public bool IsLoginMode => Mode == AuthMode.Login;
    public bool IsSignUpMode => Mode == AuthMode.SignUp;
    public bool IsChangePasswordMode => Mode == AuthMode.Change;
    public bool IsGuestMode => Mode == AuthMode.Guest;

    public string Title =>
        IsLoginMode ? "Log In" :
        IsSignUpMode ? "Sign Up" :
        IsChangePasswordMode ? "Change Password" :
        IsGuestMode ? "Guest Mode" :
        "Unknown";

    public List<string> GenderOptions { get; } = new() { "Male", "Female", "Other" };

    public string Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
            OnPropertyChanged(nameof(CanRegister));
            _registerCommand?.RaiseCanExecuteChanged();
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            SetProperty(ref _email, value);
            OnPropertyChanged(nameof(CanLogin));
            OnPropertyChanged(nameof(CanRegister));
            OnPropertyChanged(nameof(CanChangePassword));
            _loginCommand?.RaiseCanExecuteChanged();
            _registerCommand?.RaiseCanExecuteChanged();
            _changePasswordCommand?.RaiseCanExecuteChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            OnPropertyChanged(nameof(CanLogin));
            OnPropertyChanged(nameof(CanRegister));
            _loginCommand?.RaiseCanExecuteChanged();
            _registerCommand?.RaiseCanExecuteChanged();
        }
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            SetProperty(ref _confirmPassword, value);
            OnPropertyChanged(nameof(CanRegister));
            OnPropertyChanged(nameof(CanChangePassword));
            _registerCommand?.RaiseCanExecuteChanged();
            _changePasswordCommand?.RaiseCanExecuteChanged();
        }
    }

    public string OldPassword
    {
        get => _oldPassword;
        set
        {
            SetProperty(ref _oldPassword, value);
            OnPropertyChanged(nameof(CanChangePassword));
            _changePasswordCommand?.RaiseCanExecuteChanged();
        }
    }

    public string NewPassword
    {
        get => _newPassword;
        set
        {
            SetProperty(ref _newPassword, value);
            OnPropertyChanged(nameof(CanChangePassword));
            _changePasswordCommand?.RaiseCanExecuteChanged();
        }
    }

    public string Gender
    {
        get => _gender;
        set
        {
            SetProperty(ref _gender, value);
            OnPropertyChanged(nameof(CanRegister));
            _registerCommand?.RaiseCanExecuteChanged();
        }
    }

    public DateTimeOffset? Birthday
    {
        get => _birthday;
        set
        {
            SetProperty(ref _birthday, value);
            OnPropertyChanged(nameof(BirthdayForPicker));
            OnPropertyChanged(nameof(CanRegister));
            _registerCommand?.RaiseCanExecuteChanged();
        }
    }

    public DateTimeOffset BirthdayForPicker
    {
        get => _birthday ?? DateTimeOffset.Now;
        set
        {
            SetProperty(ref _birthday, value);
            OnPropertyChanged(nameof(Birthday));
            OnPropertyChanged(nameof(CanRegister));
            _registerCommand?.RaiseCanExecuteChanged();
        }
    }

    public string? AvatarUri
    {
        get => _avatarUri;
        set
        {
            SetProperty(ref _avatarUri, value);
            OnPropertyChanged(nameof(CanRegister));
            _registerCommand?.RaiseCanExecuteChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            SetProperty(ref _errorMessage, value);
            OnPropertyChanged(nameof(ErrorMessageVisibility));
        }
    }

    public bool ErrorMessageVisibility => !string.IsNullOrEmpty(ErrorMessage);

    public bool CanLogin
    {
        get
        {
            bool canLogin = !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
            return canLogin;
        }
    }

    public bool CanRegister
    {
        get
        {
            bool canRegister = !string.IsNullOrWhiteSpace(Name) &&
                               !string.IsNullOrWhiteSpace(Email) &&
                               !string.IsNullOrWhiteSpace(Password) &&
                               !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                               !string.IsNullOrWhiteSpace(Gender) &&
                               Birthday.HasValue &&
                               Password == ConfirmPassword;
            return canRegister;
        }
    }

    public bool CanChangePassword
    {
        get
        {
            bool canChangePassword = !string.IsNullOrWhiteSpace(Email) &&
                                     !string.IsNullOrWhiteSpace(NewPassword) &&
                                     !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                                     NewPassword == ConfirmPassword;
            return canChangePassword;
        }
    }

    public DataTemplate CurrentTemplate
    {
        get => _currentTemplate;
        set => SetProperty(ref _currentTemplate, value);
    }

    private void UpdateCurrentTemplate()
    {
        CurrentTemplate = Mode switch
        {
            AuthMode.Login => (DataTemplate)Application.Current.Resources["LoginTemplate"],
            AuthMode.SignUp => (DataTemplate)Application.Current.Resources["SignUpTemplate"],
            AuthMode.Change => (DataTemplate)Application.Current.Resources["ChangeTemplate"],
            AuthMode.Guest => (DataTemplate)Application.Current.Resources["GuestTemplate"],
            _ => (DataTemplate)Application.Current.Resources["LoginTemplate"]
        };
    }

    public ICommand GoLoginCommand => _goLoginCommand;
    public ICommand GoSignUpCommand => _goSignUpCommand;
    public ICommand GoChangePasswordCommand => _goChangePasswordCommand;
    public ICommand GoGuestModeCommand => _goGuestModeCommand;
    public ICommand UploadAvatarCommand => _uploadAvatarCommand;
    public ICommand LoginCommand => _loginCommand;
    public ICommand RegisterCommand => _registerCommand;
    public ICommand ChangePasswordCommand => _changePasswordCommand;

    private async void ExecuteLogin()
    {
        try
        {
            ErrorMessage = string.Empty;
            var user = await _userDataService.LoginUserAsync(Email, Password);
            if (user != null)
            {
                CurrentUser = user;
                _navigationService.NavigateTo(typeof(MainPage));
            }
            else
            {
                ErrorMessage = "Invalid email or password";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Log in failed: {ex.Message}";
        }
    }

    private async void ExecuteRegister()
    {
        try
        {
            ErrorMessage = string.Empty;
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                return;
            }
            var user = new User
            {
                Name = Name,
                Email = Email,
                Password = Password,
                Gender = Gender,
                Birthday = Birthday,
                AvatarUri = AvatarUri
            };
            await _userDataService.RegisterUserAsync(user, Password);
            CurrentUser = user;
            _navigationService.NavigateTo(typeof(MainPage));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Registration failed: {ex.Message}";
        }
    }

    private async void ExecuteChangePassword()
    {
        try
        {
            ErrorMessage = string.Empty;
            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "New password and confirmation do not match";
                return;
            }
            await _userDataService.ResetPasswordAsync(Email, NewPassword);
            ErrorMessage = "Password changed successfully";
            if (CurrentUser?.Email == Email)
            {
                CurrentUser.Password = NewPassword;
            }
            _navigationService.NavigateTo(typeof(MainPage));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Password change failed: {ex.Message}";
        }
    }

    private async void ExecuteUploadAvatar()
    {
        try
        {
            ErrorMessage = string.Empty;

            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var mainWindow = ((App)Application.Current).MainWindow;
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                ErrorMessage = "No file selected";
                return;
            }

            string extension = Path.GetExtension(file.Name).ToLower();
            if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
            {
                ErrorMessage = "Only PNG, JPEG or JPG files are allowed";
                return;
            }

            AvatarUri = file.Path;

            if (CurrentUser == null)
            {
                ErrorMessage = "Avatar uploaded but not saved due to Guest Mode. Log in to save changes";
            }
            else
            {
                if (CurrentUser != null)
                {
                    CurrentUser.AvatarUri = AvatarUri;
                    await _userDataService.UpdateUserAsync(CurrentUser);
                    ErrorMessage = "Avatar uploaded and saved successfully";
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to upload avatar: {ex.Message}";
        }
    }

    public async Task NavigateToMainPage()
    {
        try
        {
            ErrorMessage = string.Empty;
            await _userDataService.SetGuestModeAsync();
            CurrentUser = null;
            _navigationService.NavigateTo(typeof(MainPage));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to enter guest mode: {ex.Message}";
        }
    }

    private async Task LoadCurrentUserAsync()
    {
        CurrentUser = await _userDataService.GetCurrentUserAsync();
    }

    private void ClearForm()
    {
        Name = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        OldPassword = string.Empty;
        NewPassword = string.Empty;
        Gender = string.Empty;
        Birthday = null;
        AvatarUri = null;
        ErrorMessage = string.Empty;
    }
}