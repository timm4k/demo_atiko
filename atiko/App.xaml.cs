using atiko.Services;
using atiko.Services.Contracts;
using atiko.ViewModels;
using atiko.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.Globalization;
using Windows.Storage;

namespace atiko;

public partial class App : Application
{
    private MainWindow? _mainWindow;
    private readonly IServiceProvider _serviceProvider;

    public IServiceProvider GetServiceProvider() => _serviceProvider;

    public MainWindow? MainWindow => _mainWindow;

    public static object? MainViewModel { get; internal set; }

    public App()
    {
        InitializeComponent();

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider()
            ?? throw new InvalidOperationException("Failed to build ServiceProvider");

        UnhandledException += OnUnhandledException;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IUserDataService, UserDataService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();

        services.AddTransient<AuthViewModel>();
        services.AddTransient<ItemDetailsViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<AddItemViewModel>();

        services.AddTransient<AuthPage>();
        services.AddTransient<ItemDetailsPage>();
        services.AddTransient<MainPage>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<AddItemPage>();

        services.AddSingleton<MainWindow>();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        var rootFrame = _mainWindow.MainFrameProperty;

        if (rootFrame == null)
        {
            rootFrame = new Frame();
            _mainWindow.MainFrameProperty = rootFrame;
        }

        _mainWindow.Content = rootFrame;
        _mainWindow.Activate();
        Debug.WriteLine($"Window activated: {_mainWindow != null}");

        var navigationService = _serviceProvider.GetRequiredService<INavigationService>();
        Debug.WriteLine("App.xaml.cs: Calling NavigationService.Initialize...");
        navigationService.Initialize(rootFrame);
        Debug.WriteLine("App.xaml.cs: NavigationService.Initialize called.");

        ConfigureTheme(rootFrame);
        ConfigureLanguage();

        if (rootFrame.Content == null)
        {
            navigationService.NavigateTo(typeof(AuthPage));
        }
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        Debug.WriteLine($"Unhandled Exception: {e.Exception.Message}");
        Debug.WriteLine($"StackTrace: {e.Exception.StackTrace}");
    }

    private static void ConfigureTheme(Frame rootFrame)
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        string? savedTheme = localSettings.Values["Theme"] as string;

        if (rootFrame is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Enum.TryParse<ElementTheme>(savedTheme, out var theme)
                ? theme
                : ElementTheme.Light;
        }
    }

    private static void ConfigureLanguage()
    {
        string? currentLanguage = LoadLanguagePreference() ?? "en-US";
        SetPrimaryLanguageOverride(currentLanguage);
    }

    private static void SetPrimaryLanguageOverride(string languageCode)
    {
        var supportedLanguages = ApplicationLanguages.ManifestLanguages;
        languageCode = supportedLanguages?.Contains(languageCode) == true
            ? languageCode
            : supportedLanguages?.FirstOrDefault() ?? "en-US";
        ApplicationLanguages.PrimaryLanguageOverride = languageCode;
    }

    private static string? LoadLanguagePreference()
    {
        return ApplicationData.Current.LocalSettings.Values["Language"] as string;
    }
}