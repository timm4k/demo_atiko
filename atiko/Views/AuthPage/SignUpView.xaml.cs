using atiko.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace atiko.Views;

public sealed partial class SignUpView : UserControl
{
    public AuthViewModel? ViewModel { get; }

    public SignUpView()
    {
        InitializeComponent();
        var serviceProvider = (App.Current as App)?.GetServiceProvider();
        ViewModel = serviceProvider?.GetService<AuthViewModel>();
        if (ViewModel == null)
        {
            Debug.WriteLine("SignUpView: Failed to get AuthViewModel from DI. DataContext may be inherited from parent.");
        }
        else
        {
            DataContext = ViewModel;
        }
    }

    private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ViewModel != null)
        {
            Debug.WriteLine($"SignUpView.NameTextBox_TextChanged: Name updated to {ViewModel.Name ?? "null"}");
        }
    }

    private void DatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
    {
        if (ViewModel != null)
        {
            Debug.WriteLine($"SignUpView.DatePicker_DateChanged: Birthday updated to {ViewModel.BirthdayForPicker}");
        }
    }

    private void GenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ViewModel != null)
        {
            Debug.WriteLine($"SignUpView.GenderComboBox_SelectionChanged: Gender updated to {ViewModel.Gender ?? "null"}");
        }
    }

    private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ViewModel != null)
        {
            Debug.WriteLine($"SignUpView.EmailTextBox_TextChanged: Email updated to {ViewModel.Email ?? "null"}");
        }
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            Debug.WriteLine("SignUpView.PasswordBox_PasswordChanged: Password updated.");
        }
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            Debug.WriteLine("SignUpView.ConfirmPasswordBox_PasswordChanged: Confirm Password updated.");
        }
    }
}