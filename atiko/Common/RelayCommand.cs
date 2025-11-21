using atiko.Models;
using System;
using System.Windows.Input;

namespace atiko.Common;

public partial class RelayCommand : ICommand
{
    private readonly Action<object?>? _execute;
    private readonly Func<object?, bool>? _canExecute;
    private Action<VintageItem?>? navigateToItemDetails;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = _ => execute();
        _canExecute = canExecute != null ? _ => canExecute() : null;
    }

    public RelayCommand(Action<VintageItem?> navigateToItemDetails)
    {
        this.navigateToItemDetails = navigateToItemDetails ?? throw new ArgumentNullException(nameof(navigateToItemDetails));
        _execute = parameter => this.navigateToItemDetails(parameter as VintageItem);
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute?.Invoke(parameter);

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public partial class RelayCommand<T>(Action<T?> execute, Func<T?, bool>? canExecute = null) : ICommand
{
    private readonly Action<T?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly Func<T?, bool>? _canExecute = canExecute;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (parameter == null && typeof(T).IsValueType)
        {
            return _canExecute?.Invoke(default) ?? true;
        }
        return parameter is T typedParameter && (_canExecute?.Invoke(typedParameter) ?? true);
    }

    public void Execute(object? parameter)
    {
        if (parameter == null && typeof(T).IsValueType)
        {
            _execute(default);
        }
        else if (parameter is T typedParameter)
        {
            _execute(typedParameter);
        }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}