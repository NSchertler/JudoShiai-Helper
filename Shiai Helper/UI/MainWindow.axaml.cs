using Avalonia.Controls;
namespace Shiai_Helper;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var loader = (ShiaiLoader)DataContext;
        loader!.Parent = this;        
    }
}