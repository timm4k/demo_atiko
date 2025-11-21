using atiko.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace atiko.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel? ViewModel { get; }

    public SettingsPage()
    {
        InitializeComponent();
        var serviceProvider = (App.Current as App)?.GetServiceProvider();
        ViewModel = serviceProvider?.GetService<SettingsViewModel>() ??
            throw new InvalidOperationException("Couldn't get SettingsViewModel from DI");
        DataContext = ViewModel;
        Loaded += SettingsPage_Loaded;
    }

    private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        var window = (App.Current as App)?.MainWindow;
        if (window != null)
        {
            Debug.WriteLine("SettingsPage.SettingsPage_Loaded: Initializing ViewModel.");
            ViewModel?.Initialize(window);
        }
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (ViewModel != null)
        {
            Debug.WriteLine("SettingsPage.OnNavigatedTo: Calling ViewModel.OnNavigatedTo");
            await ViewModel.OnNavigatedTo();
        }
    }

    private void NavigateBack_Click(object sender, RoutedEventArgs e)
    {
        if (IsWindows10_1809OrLater() && Frame != null && Frame.CanGoBack)
        {
            Debug.WriteLine("SettingsPage.NavigateBack_Click: Navigating back.");
            Frame.GoBack();
        }
        else
        {
            Debug.WriteLine("SettingsPage.NavigateBack_Click: Cannot navigate back - invalid frame or OS version.");
        }
    }

    private void SettingsPanel_Loaded(object sender, RoutedEventArgs e)
    {
        if (IsWindows10_1809OrLater() && Resources.TryGetValue("FadeInAnimation", out var resource) && resource is Storyboard storyboard)
        {
            Debug.WriteLine("SettingsPage.SettingsPanel_Loaded: Starting FadeInAnimation.");
            storyboard.Begin();
        }
        else
        {
            Debug.WriteLine("SettingsPage.SettingsPanel_Loaded: FadeInAnimation not started - OS or resource issue.");
        }
    }

    private async void Logout_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            Debug.WriteLine($"SettingsPage.Logout_Click: Logout clicked. IsGuestMode: {ViewModel.IsGuestMode}");
            await ViewModel.LogoutAsync();
        }
    }

    private void ChangePassword_Click(object sender, RoutedEventArgs e)
    {
        if (IsWindows10_1809OrLater() && Frame != null && ViewModel != null && ViewModel.CurrentUser != null)
        {
            Debug.WriteLine("SettingsPage.ChangePassword_Click: Navigating to AuthPage for password change.");
            Frame.Navigate(typeof(AuthPage), AuthViewModel.AuthMode.Change);
        }
        else
        {
            Debug.WriteLine("SettingsPage.ChangePassword_Click: Cannot navigate to password change: Invalid frame or not logged in.");
        }
    }

    private static bool IsWindows10_1809OrLater()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
               Environment.OSVersion.Version >= new Version(10, 0, 17763, 0);
    }
}