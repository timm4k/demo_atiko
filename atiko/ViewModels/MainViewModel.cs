using atiko.Common;
using atiko.Models;
using atiko.Services.Contracts;
using atiko.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace atiko.ViewModels;

public class EraFilters : INotifyPropertyChanged
{
    private bool _pre1900s, _nineteenHundredsToFifties, _fiftiesToSeventies, _seventiesToNineties;
    public bool Pre1900s { get => _pre1900s; set { _pre1900s = value; OnPropertyChanged(); } }
    public bool NineteenHundredsToFifties { get => _nineteenHundredsToFifties; set { _nineteenHundredsToFifties = value; OnPropertyChanged(); } }
    public bool FiftiesToSeventies { get => _fiftiesToSeventies; set { _fiftiesToSeventies = value; OnPropertyChanged(); } }
    public bool SeventiesToNineties { get => _seventiesToNineties; set { _seventiesToNineties = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class ConditionFilters : INotifyPropertyChanged
{
    private bool _all = true, _mint, _excellent, _good, _fair, _poor;
    public bool All
    {
        get => _all;
        set
        {
            if (_all != value)
            {
                _all = value;
                OnPropertyChanged();
                if (value) Mint = Excellent = Good = Fair = Poor = false;
            }
        }
    }
    public bool Mint { get => _mint; set { if (_mint != value) { _mint = value; OnPropertyChanged(); if (value) All = false; } } }
    public bool Excellent { get => _excellent; set { if (_excellent != value) { _excellent = value; OnPropertyChanged(); if (value) All = false; } } }
    public bool Good { get => _good; set { if (_good != value) { _good = value; OnPropertyChanged(); if (value) All = false; } } }
    public bool Fair { get => _fair; set { if (_fair != value) { _fair = value; OnPropertyChanged(); if (value) All = false; } } }
    public bool Poor { get => _poor; set { if (_poor != value) { _poor = value; OnPropertyChanged(); if (value) All = false; } } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authService;
    private ObservableCollection<VintageItem> _items = new();
    private ObservableCollection<VintageItem> _originalItems = new();
    private ObservableCollection<string> _tags = new();
    private string? _newTagText;
    private int _selectedAcquisitionDateIndex;

    public ObservableCollection<VintageItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public ObservableCollection<string> Tags { get => _tags; set => SetProperty(ref _tags, value); }
    public string? NewTagText { get => _newTagText; set => SetProperty(ref _newTagText, value); }
    public int SelectedAcquisitionDateIndex { get => _selectedAcquisitionDateIndex; set => SetProperty(ref _selectedAcquisitionDateIndex, value); }
    public EraFilters EraFilters { get; } = new();
    public ConditionFilters ConditionFilters { get; } = new() { All = true };
    public bool IsLoggedIn => _authService.CurrentUser != null;

    public ICommand AddTagCommand { get; }
    public ICommand RemoveTagCommand { get; }
    public ICommand ResetAllCommand { get; }
    public ICommand ShowAddItemCommand { get; }
    public ICommand SelectCategoryCommand { get; }
    public ICommand NavigateToProfileCommand { get; }

    public MainViewModel(INavigationService navigationService, IAuthenticationService authService) : base(navigationService)
    {
        _navigationService = navigationService;
        _authService = authService;

        AddTagCommand = new RelayCommand(AddTag, CanAddTag);
        RemoveTagCommand = new RelayCommand<string>(RemoveTag);
        ResetAllCommand = new RelayCommand(ResetAll);
        ShowAddItemCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(AddItemPage)));
        SelectCategoryCommand = new RelayCommand<string>(SelectCategory);
        NavigateToProfileCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(SettingsPage)));

        EraFilters.PropertyChanged += (_, __) => ApplyFilters();
        ConditionFilters.PropertyChanged += (_, __) => ApplyFilters();
    }

    private void ApplyFilters()
    {
        Items = new ObservableCollection<VintageItem>(
            _originalItems.Where(item =>
            {
                int year = item.Year ?? 0;
                bool eraOk = true;
                if (EraFilters.Pre1900s || EraFilters.NineteenHundredsToFifties ||
                    EraFilters.FiftiesToSeventies || EraFilters.SeventiesToNineties)
                {
                    eraOk =
                        (EraFilters.Pre1900s && year < 1900) ||
                        (EraFilters.NineteenHundredsToFifties && year >= 1900 && year <= 1959) ||
                        (EraFilters.FiftiesToSeventies && year >= 1950 && year <= 1979) ||
                        (EraFilters.SeventiesToNineties && year >= 1970 && year <= 1999);
                }
                bool conditionOk = ConditionFilters.All ||
                                   (ConditionFilters.Mint && item.Condition == "Mint") ||
                                   (ConditionFilters.Excellent && item.Condition == "Excellent") ||
                                   (ConditionFilters.Good && item.Condition == "Good") ||
                                   (ConditionFilters.Fair && item.Condition == "Fair") ||
                                   (ConditionFilters.Poor && item.Condition == "Poor");
                return eraOk && conditionOk;
            })
        );
    }

    private void ResetAll()
    {
        EraFilters.Pre1900s = EraFilters.NineteenHundredsToFifties =
        EraFilters.FiftiesToSeventies = EraFilters.SeventiesToNineties = false;
        ConditionFilters.All = true;
        SelectedAcquisitionDateIndex = 0;
        ApplyFilters();
    }

    private void SelectCategory(string? category)
    {
        Items = category == "All"
            ? new ObservableCollection<VintageItem>(_originalItems)
            : new ObservableCollection<VintageItem>(_originalItems.Where(x => x.Category == category));
    }

    private bool CanAddTag() => !string.IsNullOrWhiteSpace(NewTagText?.Trim()) &&
                                !Tags.Contains(NewTagText!.Trim(), StringComparer.OrdinalIgnoreCase);

    private async void AddTag()
    {
        var tag = NewTagText?.Trim();
        if (!string.IsNullOrWhiteSpace(tag) && !Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            Tags.Add(tag);
            NewTagText = null;
            await SaveTagsAsync();
        }
    }

    private async void RemoveTag(string? tag)
    {
        if (tag != null && Tags.Remove(tag))
            await SaveTagsAsync();
    }

    public async Task LoadItemsAsync()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Assets/Data/VintageItems.json");
        if (!File.Exists(path)) return;
        string json = await File.ReadAllTextAsync(path);
        var list = JsonSerializer.Deserialize<List<VintageItem>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (list != null)
        {
            _originalItems = new ObservableCollection<VintageItem>(list);
            Items = new ObservableCollection<VintageItem>(list);
            ApplyFilters();
        }
    }

    public async Task LoadTagsAsync()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Assets/Data/Tags.json");
        if (File.Exists(path))
        {
            string json = await File.ReadAllTextAsync(path);
            var list = JsonSerializer.Deserialize<List<string>>(json);
            Tags = list != null ? new ObservableCollection<string>(list) : new();
        }
        else
        {
            Tags = new ObservableCollection<string> { "Vintage", "Antique", "Collectible" };
            await SaveTagsAsync();
        }
    }

    public async Task SaveTagsAsync()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Assets/Data/Tags.json");
        string json = JsonSerializer.Serialize(Tags.ToList(), new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
    }

    public async void AddItem(VintageItem item)
    {
        _originalItems.Add(item);
        ApplyFilters();
        string path = Path.Combine(AppContext.BaseDirectory, "Assets/Data/VintageItems.json");
        string json = JsonSerializer.Serialize(_originalItems.ToList(), new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
    }
}