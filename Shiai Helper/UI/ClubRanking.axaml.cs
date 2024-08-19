using Avalonia.Controls;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
using Shiai_Helper.Models;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using System.Collections;
using System.Linq;
using Border = MigraDoc.DocumentObjectModel.Border;
using System.Windows.Input;
using ReactiveUI;
using System;
using Avalonia.Metadata;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using DynamicData;
using Shiai_Helper.PDF;
using Avalonia.VisualTree;
using Avalonia.LogicalTree;
using System.Threading.Tasks;

namespace Shiai_Helper.UI
{
    public partial class ClubRanking : TournamentBasedUserControlBase<ClubRankingViewModel>
    {            
        public ClubRanking()
        { 
            InitializeComponent();

            vm = (ClubRankingViewModel)layoutRoot.DataContext!;
        }
    }

    public class ClubRankingViewModel : ReactiveObject, ITournamentBasedViewModel
    {
        private Tournament? tournament;
        public Tournament? Tournament 
        {
            get => tournament;
            set
            {
                this.RaiseAndSetIfChanged(ref tournament, value);
                Calculate();
            }
        }

        public ObservableCollection<int> PointsForPlace { get; } = new ObservableCollection<int>();

        ClubRankingCalculator rankingCalculator = new ClubRankingCalculator();
        ClubRankingOptions rankingOptions = new ClubRankingOptions();

        private Models.ClubRanking? clubRanking;
        public Models.ClubRanking? ClubRanking
        {
            get => clubRanking;
            set => this.RaiseAndSetIfChanged(ref clubRanking, value);
        }

        public ObservableCollection<string> CategoriesNotDrawn { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> CategoriesInProgress { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> CategoriesAllFought { get; } = new ObservableCollection<string>();        
        public ObservableCollection<string> CategoriesFinished { get; } = new ObservableCollection<string>();
        public Window? Window { get; set; }

        public ClubRankingViewModel()
        {
            PointsForPlace.Add(10);
            PointsForPlace.Add(7);
            PointsForPlace.Add(5);
            PointsForPlace.Add(3);
            PointsForPlace.Add(0);

            PointsForPlace.CollectionChanged += (s, e) => PointsForPlaceChanged();
            PointsForPlaceChanged();
        }

        public void Calculate()
        {
            FillCategoryList(CategoryState.NotDrawn, CategoriesNotDrawn);
            FillCategoryList(CategoryState.InProgress, CategoriesInProgress);
            FillCategoryList(CategoryState.FinishedNoWinners, CategoriesAllFought);
            FillCategoryList(CategoryState.Finished, CategoriesFinished);

            if (Tournament != null)
                ClubRanking = rankingCalculator.CalculateClubRanking(Tournament, rankingOptions);
            else
                ClubRanking = null;
        }

        private void FillCategoryList(CategoryState categoryState, ObservableCollection<string> targetList)
        {
            targetList.Clear();
            if(Tournament != null)
                targetList.AddRange(Tournament.Categories.Where(c => c.Value.State == categoryState).OrderBy(c => c.Value.Name).Select(c => c.Value.Name));
        }

        private void PointsForPlaceChanged()
        {
            rankingOptions.SetAwardedPointsForPlace(1, PointsForPlace[0]);
            rankingOptions.SetAwardedPointsForPlace(2, PointsForPlace[1]);
            rankingOptions.SetAwardedPointsForPlace(3, PointsForPlace[2]);
            rankingOptions.SetAwardedPointsForPlace(5, PointsForPlace[3]);
            rankingOptions.SetAwardedPointsForPlace(7, PointsForPlace[4]);

            Calculate();
        }

        public async void CreatePdf()
        {
            if (Tournament == null)
                return;
                       
            await CreateRankingPdf(Tournament);
        }

        [DependsOn(nameof(Tournament))]
        public bool CanCreatePdf(object parameter)
        {
            return Tournament != null;
        }


        async Task CreateRankingPdf(Tournament tournament)
        {
            if (ClubRanking == null)
                throw new InvalidOperationException("The club ranking has not been calculated yet.");
            if (Window == null)
                throw new InvalidOperationException("This view model is not attached to a window.");

            var sfd = new SaveFileDialog();
            sfd.Filters = [new FileDialogFilter() { Name = "PDF-Datei", Extensions = new List<string>() { "pdf" } }];
            sfd.InitialFileName = "Vereinswertung.pdf";
            var filename = await sfd.ShowAsync(Window);
            if (filename == null)
                return;

            try
            {
                ClubRankingPdf.Create(ClubRanking, $"Vereinswertung {tournament.Name}", filename);

                Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
            }
            catch(Exception x)
            {
                var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                  .GetMessageBoxStandardWindow("Fehler", "Beim Erstellen der PDF ist ein Fehler aufgetreten: " + x.Message,
                                                MessageBox.Avalonia.Enums.ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error);
                await messageBoxStandardWindow.ShowDialog(Window);
            }            
        }
    }

}
