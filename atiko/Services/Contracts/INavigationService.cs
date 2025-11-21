using Microsoft.UI.Xaml.Controls;
using System;

namespace atiko.Services.Contracts;

public interface INavigationService
{
    void Initialize(Frame frame);
    void NavigateTo(Type pageType, object? parameter = null);
    void NavigateTo(string pageName);
    bool CanGoBack { get; }
    void GoBack();
    void ClearBackStack();
    void NavigateWithSlide(Type pageType, object? parameter = null);
    void NavigateWithFade(Type pageType, object? parameter = null);
}