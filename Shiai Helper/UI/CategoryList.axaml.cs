using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using System;

namespace Shiai_Helper.UI;

public partial class CategoryList : ItemsControl, IStyleable
{
    public static readonly StyledProperty<IBrush> ItemBackgroundProperty = AvaloniaProperty.Register<CategoryList, IBrush>(nameof(ItemBackground), new SolidColorBrush(Colors.Gray));

    public IBrush ItemBackground
    {
        get => GetValue(ItemBackgroundProperty);
        set => SetValue(ItemBackgroundProperty, value);
    }

    Type IStyleable.StyleKey => typeof(ItemsControl);

    public CategoryList()
    {
        InitializeComponent();
    }
}