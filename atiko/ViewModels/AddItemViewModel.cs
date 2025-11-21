using atiko.Common;
using atiko.Models;
using atiko.Services.Contracts;
using atiko.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace atiko.ViewModels;

public partial class AddItemViewModel : ViewModelBase, IDisposable
{
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private VintageItem _newItem = new();
    private string _yearInput = "";
    private string _valueInput = "";
    private bool _isYearInvalid;
    private bool _isValueInvalid;
    private string _yearErrorMessage = "";
    private string _valueErrorMessage = "";
    private bool _isAcquisitionDateInvalid;
    private string _acquisitionDateErrorMessage = "";

    public AddItemViewModel(INavigationService navigationService, IServiceProvider serviceProvider) : base(navigationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        CloseAddItemPanelCommand = new RelayCommand(CloseAddItemPanel);
        AddItemCommand = new RelayCommand(AddItem, CanAddItem);
        CancelAddCommand = new RelayCommand(ExecuteCancelAdd);
        UploadImageCommand = new RelayCommand(async () => await ExecuteUploadImageAsync());
    }

    public VintageItem NewItem
    {
        get => _newItem;
        set => SetProperty(ref _newItem, value);
    }

    public DateTimeOffset? AcquisitionDateForPicker
    {
        get => NewItem.AcquisitionDate != default ? new DateTimeOffset(NewItem.AcquisitionDate) : null;
        set
        {
            DateTimeOffset minDate = NewItem.Year != null ? new DateTimeOffset(new DateTime(NewItem.Year.Value, 1, 1)) : DateTimeOffset.MinValue;
            DateTimeOffset maxDate = DateTimeOffset.Now;

            if (value != null && value.Value < minDate)
            {
                IsAcquisitionDateInvalid = true;
                AcquisitionDateErrorMessage = $"Date cannot be earlier than production year ({minDate:yyyy})";
                NewItem.AcquisitionDate = minDate.DateTime;
            }
            else if (value != null && value.Value > maxDate)
            {
                IsAcquisitionDateInvalid = true;
                AcquisitionDateErrorMessage = $"Date cannot be later than today ({maxDate:yyyy-MM-dd})";
                NewItem.AcquisitionDate = maxDate.DateTime;
            }
            else
            {
                IsAcquisitionDateInvalid = false;
                AcquisitionDateErrorMessage = "";
                NewItem.AcquisitionDate = value?.DateTime ?? default;
            }
            OnPropertyChanged(nameof(AcquisitionDateForPicker));
            OnPropertyChanged(nameof(CanAddItem));
        }
    }

    public string YearInput
    {
        get => _yearInput;
        set
        {
            if (SetProperty(ref _yearInput, value ?? ""))
            {
                if (string.IsNullOrWhiteSpace(_yearInput))
                {
                    NewItem.Year = null;
                    IsYearInvalid = false;
                    YearErrorMessage = "";
                }
                else if (int.TryParse(_yearInput, out int year))
                {
                    if (year < 1800 || year > 2025)
                    {
                        IsYearInvalid = true;
                        YearErrorMessage = "Year must be between 1800 and 2025";
                    }
                    else
                    {
                        NewItem.Year = year;
                        IsYearInvalid = false;
                        YearErrorMessage = "";
                    }
                }
                else
                {
                    IsYearInvalid = true;
                    YearErrorMessage = "Please enter numbers only";
                }
                OnPropertyChanged(nameof(CanAddItem));
            }
        }
    }

    public string ValueInput
    {
        get => _valueInput;
        set
        {
            if (SetProperty(ref _valueInput, value ?? ""))
            {
                if (string.IsNullOrWhiteSpace(_valueInput))
                {
                    NewItem.Value = 0;
                    IsValueInvalid = false;
                    ValueErrorMessage = "";
                }
                else if (decimal.TryParse(_valueInput, out decimal result))
                {
                    if (result < 0)
                    {
                        IsValueInvalid = true;
                        ValueErrorMessage = "Value cannot be negative";
                    }
                    else
                    {
                        NewItem.Value = result;
                        IsValueInvalid = false;
                        ValueErrorMessage = "";
                    }
                }
                else
                {
                    IsValueInvalid = true;
                    ValueErrorMessage = "Please enter numbers only";
                }
                OnPropertyChanged(nameof(CanAddItem));
            }
        }
    }

    public bool IsYearInvalid
    {
        get => _isYearInvalid;
        set => SetProperty(ref _isYearInvalid, value);
    }

    public bool IsValueInvalid
    {
        get => _isValueInvalid;
        set => SetProperty(ref _isValueInvalid, value);
    }

    public string YearErrorMessage
    {
        get => _yearErrorMessage;
        set => SetProperty(ref _yearErrorMessage, value);
    }

    public string ValueErrorMessage
    {
        get => _valueErrorMessage;
        set => SetProperty(ref _valueErrorMessage, value);
    }

    public bool IsAcquisitionDateInvalid
    {
        get => _isAcquisitionDateInvalid;
        set => SetProperty(ref _isAcquisitionDateInvalid, value);
    }

    public string AcquisitionDateErrorMessage
    {
        get => _acquisitionDateErrorMessage;
        set => SetProperty(ref _acquisitionDateErrorMessage, value);
    }

    public ICommand CloseAddItemPanelCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand CancelAddCommand { get; }
    public ICommand UploadImageCommand { get; }

    private void CloseAddItemPanel()
    {
        _navigationService.GoBack();
    }

    public void AddItem()
    {
        if (string.IsNullOrEmpty(NewItem.Id))
            NewItem.Id = Guid.NewGuid().ToString();
        if (NewItem.AddedDate == default)
            NewItem.AddedDate = DateTime.Now;

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.AddItem(NewItem);
        _navigationService.NavigateTo(typeof(ItemDetailsPage), NewItem);
    }

    public bool CanAddItem()
    {
        return !string.IsNullOrWhiteSpace(NewItem.Name) &&
               NewItem.Year.HasValue &&
               !string.IsNullOrWhiteSpace(NewItem.Category) &&
               !string.IsNullOrWhiteSpace(NewItem.Condition) &&
               !string.IsNullOrWhiteSpace(NewItem.Description) &&
               NewItem.Value > 0 &&
               NewItem.AcquisitionDate != default &&
               !string.IsNullOrWhiteSpace(NewItem.Source) &&
               !string.IsNullOrWhiteSpace(NewItem.ImageUri) &&
               !string.IsNullOrWhiteSpace(NewItem.Notes) &&
               !IsYearInvalid &&
               !IsValueInvalid &&
               !IsAcquisitionDateInvalid;
    }

    private void ExecuteCancelAdd()
    {
        _navigationService.GoBack();
    }

    private async Task ExecuteUploadImageAsync()
    {
        try
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var mainWindow = ((App)Application.Current).MainWindow;
            if (mainWindow == null)
                return;

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file == null)
                return;

            var localFolder = ApplicationData.Current.LocalFolder;
            var storedFile = await file.CopyAsync(localFolder, file.Name, NameCollisionOption.GenerateUniqueName);
            NewItem.ImageUri = storedFile.Path;
            OnPropertyChanged(nameof(NewItem));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Image upload error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}