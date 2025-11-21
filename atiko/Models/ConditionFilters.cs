using atiko.Common;

namespace atiko.Models;

public partial class ConditionFilters : ViewModelBase
{
    private bool _all;
    private bool _mint;
    private bool _excellent;
    private bool _good;
    private bool _fair;
    private bool _poor;

    public ConditionFilters() : base()
    {
    }

    public bool All
    {
        get => _all;
        set => SetProperty(ref _all, value);
    }

    public bool Mint
    {
        get => _mint;
        set => SetProperty(ref _mint, value);
    }

    public bool Excellent
    {
        get => _excellent;
        set => SetProperty(ref _excellent, value);
    }

    public bool Good
    {
        get => _good;
        set => SetProperty(ref _good, value);
    }

    public bool Fair
    {
        get => _fair;
        set => SetProperty(ref _fair, value);
    }

    public bool Poor
    {
        get => _poor;
        set => SetProperty(ref _poor, value);
    }

    public bool AllConditionsSelected()
    {
        return All || (!Mint && !Excellent && !Good && !Fair && !Poor);
    }
}