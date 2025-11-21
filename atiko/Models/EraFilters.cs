using atiko.Common;

namespace atiko.Models;

public partial class EraFilters : ViewModelBase
{
    private bool _pre1900s;
    private bool _nineteenHundredsToFifties;
    private bool _fiftiesToSeventies;
    private bool _seventiesToNineties;

    public EraFilters() : base()
    {
    }

    public bool Pre1900s
    {
        get => _pre1900s;
        set => SetProperty(ref _pre1900s, value);
    }

    public bool NineteenHundredsToFifties
    {
        get => _nineteenHundredsToFifties;
        set => SetProperty(ref _nineteenHundredsToFifties, value);
    }

    public bool FiftiesToSeventies
    {
        get => _fiftiesToSeventies;
        set => SetProperty(ref _fiftiesToSeventies, value);
    }

    public bool SeventiesToNineties
    {
        get => _seventiesToNineties;
        set => SetProperty(ref _seventiesToNineties, value);
    }

    public bool AllErasSelected()
    {
        return !Pre1900s && !NineteenHundredsToFifties && !FiftiesToSeventies && !SeventiesToNineties;
    }
}