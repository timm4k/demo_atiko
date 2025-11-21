using atiko.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace atiko.Views;

public sealed partial class GuestMode : UserControl
{
    public GuestMode()
    {
        this.InitializeComponent();
    }

    private async void EnterGuestModeButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthViewModel viewModel)
        {
            viewModel.ErrorMessage = string.Empty;
            await viewModel.NavigateToMainPage();
        }
    }
}