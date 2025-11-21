using atiko.ViewModels;
using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters;

public partial class ModeToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is AuthViewModel.AuthMode mode)
        {
            return mode switch
            {
                AuthViewModel.AuthMode.Login => "Log In",
                AuthViewModel.AuthMode.SignUp => "Sign Up",
                AuthViewModel.AuthMode.Change => "Change Password",
                AuthViewModel.AuthMode.Guest => "Guest Mode",
                _ => "Unknown"
            };
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}