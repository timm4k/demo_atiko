using atiko.Common;
using atiko.Models;
using atiko.Services.Contracts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace atiko.ViewModels;

public partial class ItemDetailsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly MainViewModel _mainViewModel;
    private readonly IAuthenticationService _authService;
    private readonly VintageItem _selectedItem;

    private static readonly string FavoritesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "atiko", "UserFavorites.json");
    private static Dictionary<string, HashSet<string>> _userFavorites = new();

    public VintageItem SelectedItem => _selectedItem;

    public ICommand NavigateBackCommand { get; }
    public ICommand AddToFavoritesCommand { get; }
    public ICommand ShareCommand { get; }

    public bool IsFavorite => _authService.CurrentUser?.Id is string userId && _userFavorites.ContainsKey(userId) && _userFavorites[userId].Contains(_selectedItem.Id);

    public bool IsItemCreator => _authService.CurrentUser?.Id == _selectedItem.CreatorId;
    public bool IsLoggedIn => _authService.CurrentUser != null;

    public bool ShowSellerContact => !IsItemCreator &&
        (!string.IsNullOrEmpty(_selectedItem.SellerContact.Telegram) ||
         !string.IsNullOrEmpty(_selectedItem.SellerContact.Instagram) ||
         !string.IsNullOrEmpty(_selectedItem.SellerContact.PhoneNumber));

    public ItemDetailsViewModel(INavigationService navigationService, MainViewModel mainViewModel, IAuthenticationService authService, VintageItem item) : base(navigationService)
    {
        _navigationService = navigationService;
        _mainViewModel = mainViewModel;
        _authService = authService;
        _selectedItem = item;

        _ = LoadFavoritesAsync();

        NavigateBackCommand = new RelayCommand(() => _navigationService.GoBack());
        AddToFavoritesCommand = new RelayCommand(ToggleFavorite);
        ShareCommand = new RelayCommand(ShareItem);
    }

    private async Task LoadFavoritesAsync()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FavoritesPath)!);
            if (File.Exists(FavoritesPath))
            {
                var json = await File.ReadAllTextAsync(FavoritesPath);
                var data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
                if (data != null)
                    _userFavorites = data.ToDictionary(kvp => kvp.Key, kvp => new HashSet<string>(kvp.Value));
            }
        }
        catch { }
        OnPropertyChanged(nameof(IsFavorite));
    }

    private async void ToggleFavorite()
    {
        var userId = _authService.CurrentUser?.Id ?? "guest";
        if (!_userFavorites.ContainsKey(userId))
            _userFavorites[userId] = new HashSet<string>();

        if (_userFavorites[userId].Contains(_selectedItem.Id))
            _userFavorites[userId].Remove(_selectedItem.Id);
        else
            _userFavorites[userId].Add(_selectedItem.Id);

        OnPropertyChanged(nameof(IsFavorite));

        var toSave = _userFavorites.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
        var json = JsonSerializer.Serialize(toSave, new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory(Path.GetDirectoryName(FavoritesPath)!);
        await File.WriteAllTextAsync(FavoritesPath, json);

        _mainViewModel.Items = new ObservableCollection<VintageItem>(_mainViewModel.Items.ToList());
    }

    private async void ShareItem()
    {
        var link = $"https://atiko.app/item/{_selectedItem.Id}";
        var dataPackage = new DataPackage();
        dataPackage.SetText(link);
        Clipboard.SetContent(dataPackage);

        if (Application.Current is App { MainWindow.Content: Frame frame } && frame.Content is Page page && page.FindName("ShareMessage") is TextBlock msg)
        {
            msg.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            msg.Visibility = Visibility.Collapsed;
        }
    }
}