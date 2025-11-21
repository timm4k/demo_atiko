using atiko.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace atiko.Views;

public sealed partial class AuthPage : Page
{
    public AuthViewModel ViewModel { get; }

    public AuthPage()
    {
        this.InitializeComponent();

        if (Application.Current is App app)
        {
            ViewModel = app.GetServiceProvider().GetRequiredService<AuthViewModel>();
        }
        else
        {
            throw new InvalidOperationException("Application is not of type App or ServiceProvider is not available");
        }

        DataContext = ViewModel;
    }
}
