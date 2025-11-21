using atiko.Models;
using atiko.Services.Contracts;
using atiko.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace atiko.Views;

public sealed partial class MainPage : Page, IDisposable
{
    private readonly MainViewModel _viewModel;
    private readonly INavigationService _navigationService;

    public MainPage()
    {
        InitializeComponent();
        var serviceProvider = (App.Current as App)?.GetServiceProvider();
        if (serviceProvider == null)
        {
            throw new InvalidOperationException("ServiceProvider is not available.");
        }
        _viewModel = serviceProvider.GetRequiredService<MainViewModel>();
        _navigationService = serviceProvider.GetRequiredService<INavigationService>();
        DataContext = _viewModel;
        Loaded += MainPage_Loaded;
        Unloaded += MainPage_Unloaded;
    }

    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (_viewModel.Items == null || _viewModel.Items.Count == 0)
        {
            await _viewModel.LoadItemsAsync();
        }
        await _viewModel.LoadTagsAsync();
        _viewModel.SelectedAcquisitionDateIndex = 0;
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            _viewModel.SelectedAcquisitionDateIndex = comboBox.SelectedIndex;
        }
    }

    private void ItemsContainer_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is VintageItem item)
        {
            System.Diagnostics.Debug.WriteLine($"Navigating to ItemDetailsPage for item: {item.Name}");
            _navigationService.NavigateTo(typeof(ItemDetailsPage), item);
        }
    }

    private void MainPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Dispose();
    }

    public void Dispose()
    {
        Loaded -= MainPage_Loaded;
        Unloaded -= MainPage_Unloaded;
        GC.SuppressFinalize(this);
    }
}