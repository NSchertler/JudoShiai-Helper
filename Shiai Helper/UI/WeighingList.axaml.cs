using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using Avalonia.Platform.Storage;
using MigraDoc.DocumentObjectModel;
using ReactiveUI;
using Shiai_Helper.Models;
using Shiai_Helper.PDF;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shiai_Helper.UI;

public partial class WeighingList : TournamentBasedUserControlBase<WeighingListViewModel>
{    
    public WeighingList()
    {
        InitializeComponent();
        vm = new WeighingListViewModel();
        layoutRoot.DataContext = vm;
    }
}

public class WeighingListViewModel : ReactiveObject, ITournamentBasedViewModel
{
    Tournament? tournament;
    public Tournament? Tournament 
    {
        get => tournament;
        set => this.RaiseAndSetIfChanged(ref tournament, value);
    }
    public TopLevel? TopLevel { get; set; }

    public PageFormat PageSize { get; set; } = PageFormat.A5;

    public Orientation Orientation { get; set; } = Orientation.Landscape;

    public PageFormat PageSizeOverview { get; set;} = PageFormat.A4;
    public Orientation OrientationOverview { get; set; } = Orientation.Portrait;

    public async void CreatePdf()
    {
        if (Tournament == null)
            return;
        if (TopLevel == null)
            throw new InvalidOperationException("This view model is not attached to a window.");

        var file = await TopLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            DefaultExtension = "pdf",
            SuggestedFileName = "Wiegeliste.pdf",
            FileTypeChoices = [FilePickerFileTypes.Pdf]
        });
        
        if (file == null)
            return;
        WeighingListPdf.Create(Tournament, file.Path.LocalPath, PageSizeOverview, OrientationOverview, PageSize, Orientation);
        Process.Start(new ProcessStartInfo(file.Path.LocalPath) { UseShellExecute = true });
    }

    [DependsOn(nameof(Tournament))]
    public bool CanCreatePdf(object parameter)
    {
        return Tournament != null;
    }
}