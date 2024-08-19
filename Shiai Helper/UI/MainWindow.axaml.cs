using Avalonia.Controls;
namespace Shiai_Helper;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var loader = DataContext as ShiaiLoader;
        if (loader != null)
            loader.Parent = this;        
    }
}