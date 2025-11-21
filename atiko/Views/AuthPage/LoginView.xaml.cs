using atiko.Common;
using atiko.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace atiko.Views;

public sealed partial class LoginView : UserControl
{
    public LoginView()
    {
        this.InitializeComponent();
    }

    private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is AuthViewModel vm)
        {
            Debug.WriteLine($"LoginView: EmailTextBox_TextChanged - Email set to: {EmailTextBox.Text}");
            vm.Email = EmailTextBox.Text;
            (vm.LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    private void LoginPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthViewModel vm)
        {
            Debug.WriteLine($"LoginView: LoginPasswordBox_PasswordChanged - Password set to: {LoginPasswordBox.Password}");
            vm.Password = LoginPasswordBox.Password;
            (vm.LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}