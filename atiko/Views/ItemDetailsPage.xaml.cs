using atiko.Models;
using atiko.Services.Contracts;
using atiko.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace atiko.Views;

public sealed partial class ItemDetailsPage : Page
{
    public ItemDetailsPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is VintageItem item)
        {
            var serviceProvider = (App.Current as App)?.GetServiceProvider();
            if (serviceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider is not available.");
            }
            DataContext = new ItemDetailsViewModel(
                serviceProvider.GetRequiredService<INavigationService>(),
                serviceProvider.GetRequiredService<MainViewModel>(),
                serviceProvider.GetRequiredService<IAuthenticationService>(),
                item);
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (Resources.TryGetValue("FadeInAnimation", out var storyboard))
        {
            (storyboard as Storyboard)?.Begin();
        }
    }
}