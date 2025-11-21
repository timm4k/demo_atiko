using atiko.Services.Contracts;
using atiko.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace atiko.Services;

public class NavigationService : INavigationService
{
    private Frame? _frame;

    public void Initialize(Frame frame)
    {
        _frame = frame ?? throw new ArgumentNullException(nameof(frame));
    }

    public void NavigateTo(Type pageType, object? parameter = null)
    {
        if (_frame == null)
        {
            throw new InvalidOperationException("Navigation frame is not initialized");
        }
        _frame.Navigate(pageType, parameter);
    }

    public void NavigateTo(string pageName)
    {
        if (_frame == null)
        {
            throw new InvalidOperationException("Navigation frame is not initialized");
        }
        switch (pageName.ToLower())
        {
            case "itemdetailspage":
                _frame.Navigate(typeof(ItemDetailsPage));
                break;
            case "settingspage":
                _frame.Navigate(typeof(SettingsPage));
                break;
            default:
                throw new ArgumentException($"Unknown page name: {pageName}");
        }
    }

    public bool CanGoBack => _frame?.CanGoBack ?? false;

    public void GoBack()
    {
        if (CanGoBack)
        {
            _frame?.GoBack();
        }
        else
        {
            throw new InvalidOperationException("Cannot navigate back because back stack is empty");
        }
    }

    public void ClearBackStack()
    {
        if (_frame != null && _frame.BackStackDepth > 0)
        {
            _frame.BackStack.Clear();
        }
    }

    public void NavigateWithSlide(Type pageType, object? parameter = null)
    {
        NavigateWithTransition(pageType, parameter, new SlideNavigationTransitionInfo());
    }

    public void NavigateWithFade(Type pageType, object? parameter = null)
    {
        NavigateWithTransition(pageType, parameter, new EntranceNavigationTransitionInfo());
    }

    private void NavigateWithTransition(Type pageType, object? parameter, NavigationTransitionInfo transitionInfo)
    {
        if (_frame == null)
        {
            throw new InvalidOperationException("Navigation frame is not initialized");
        }
        _frame.Navigate(pageType, parameter, transitionInfo);
    }
}