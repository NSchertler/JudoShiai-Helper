using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
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
    public Window? Window { get; set; }

    public PageFormat PageSize { get; set; } = PageFormat.A5;

    public Orientation Orientation { get; set; } = Orientation.Portrait;

    public async void CreatePdf()
    {
        if (Tournament == null)
            return;
        if (Window == null)
            throw new InvalidOperationException("This view model is not attached to a window.");

        var sfd = new SaveFileDialog();
        sfd.Filters = [new FileDialogFilter() { Name = "PDF-Datei", Extensions = new List<string>() { "pdf" } }];
        sfd.InitialFileName = "Wiegeliste.pdf";
        var path = await sfd.ShowAsync(Window);
        if (path == null)
            return;
        WeighingListPdf.Create(Tournament, path, PageSize, Orientation);
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }

    [DependsOn(nameof(Tournament))]
    public bool CanCreatePdf(object parameter)
    {
        return Tournament != null;
    }
}