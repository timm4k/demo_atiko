using atiko.Models;
using atiko.Services.Contracts;
using atiko.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace atiko.Views
{
    public sealed partial class AddItemPage : Page, IDisposable
    {
        private readonly AddItemViewModel _viewModel;
        private readonly INavigationService _navigationService;

        public AddItemPage()
        {
            this.InitializeComponent();

            var serviceProvider = (App.Current as App)?.GetServiceProvider();
            if (serviceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider is not available.");
            }

            _viewModel = serviceProvider.GetService<AddItemViewModel>()
                ?? throw new InvalidOperationException("Could not retrieve AddItemViewModel from DI");
            _navigationService = serviceProvider.GetRequiredService<INavigationService>()
                ?? throw new InvalidOperationException("Could not retrieve INavigationService from DI");

            this.DataContext = _viewModel;
            Loaded += AddItemPage_Loaded;
            Unloaded += AddItemPage_Unloaded;

            if (_viewModel.NewItem == null)
            {
                _viewModel.NewItem = new VintageItem();
            }
        }

        private void AddItemPage_Loaded(object sender, RoutedEventArgs e)
        {
            var newItem = _viewModel.NewItem ?? new VintageItem();
            newItem.Name ??= string.Empty;
            newItem.Description ??= string.Empty;
            newItem.ImageUri ??= string.Empty;
            newItem.Category ??= string.Empty;
            newItem.Condition ??= string.Empty;
            newItem.Year ??= null;
            newItem.Value = newItem.Value == 0 ? 0 : newItem.Value;
            newItem.Source ??= string.Empty;
            newItem.Notes ??= string.Empty;
            newItem.IsFavorite = false;

            if (newItem.SellerContact == null)
            {
                newItem.SellerContact = new SellerContact
                {
                    Telegram = string.Empty,
                    Instagram = string.Empty,
                    PhoneNumber = string.Empty
                };
            }

            _viewModel.NewItem = newItem;
        }

        private void AddItemPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            Loaded -= AddItemPage_Loaded;
            Unloaded -= AddItemPage_Unloaded;
            _viewModel?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
