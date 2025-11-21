using atiko.Common;
using atiko.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace atiko.Views;

public sealed partial class ChangePassword : UserControl
{
    public ChangePassword()
    {
        this.InitializeComponent();
    }

    private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is AuthViewModel vm)
        {
            vm.Email = EmailTextBox.Text;
            (vm.ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthViewModel vm)
        {
            vm.NewPassword = NewPasswordBox.Password;
            (vm.ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthViewModel vm)
        {
            vm.ConfirmPassword = ConfirmPasswordBox.Password;
            (vm.ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}