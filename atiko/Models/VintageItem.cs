using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace atiko.Models;

public class SellerContact : INotifyPropertyChanged
{
    private string _telegram = "";
    private string _instagram = "";
    private string _phoneNumber = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Telegram
    {
        get => _telegram;
        set => SetProperty(ref _telegram, value);
    }

    public string Instagram
    {
        get => _instagram;
        set => SetProperty(ref _instagram, value);
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

public class VintageItem : INotifyPropertyChanged
{
    private string _id = "";
    private string _name = "";
    private string _description = "";
    private int? _year;
    private string _category = "";
    private string _condition = "";
    private string _imageUri = "";
    private decimal _value;
    private DateTime _acquisitionDate;
    private DateTime _addedDate;
    private string _creatorId = "";
    private SellerContact _sellerContact = new();
    private string _source = "";
    private string _notes = "";
    private string _specifications = "";
    private bool _isFavorite;
    private List<string> _tags = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public int? Year
    {
        get => _year;
        set => SetProperty(ref _year, value);
    }

    public string Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public string Condition
    {
        get => _condition;
        set => SetProperty(ref _condition, value);
    }

    public string ImageUri
    {
        get => _imageUri;
        set => SetProperty(ref _imageUri, value);
    }

    public decimal Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    public DateTime AcquisitionDate
    {
        get => _acquisitionDate;
        set => SetProperty(ref _acquisitionDate, value);
    }

    public DateTime AddedDate
    {
        get => _addedDate;
        set => SetProperty(ref _addedDate, value);
    }

    public string CreatorId
    {
        get => _creatorId;
        set => SetProperty(ref _creatorId, value);
    }

    public SellerContact SellerContact
    {
        get => _sellerContact;
        set => SetProperty(ref _sellerContact, value);
    }

    public string Source
    {
        get => _source;
        set => SetProperty(ref _source, value);
    }

    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public string Specifications
    {
        get => _specifications;
        set => SetProperty(ref _specifications, value);
    }

    public bool IsFavorite
    {
        get => _isFavorite;
        set => SetProperty(ref _isFavorite, value);
    }

    public List<string> Tags
    {
        get => _tags;
        set => SetProperty(ref _tags, value);
    }

    protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}