using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace atiko;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        MainFrameProperty = new Frame();
        this.Content = MainFrameProperty;
    }

    public Frame MainFrameProperty { get; set; }
}