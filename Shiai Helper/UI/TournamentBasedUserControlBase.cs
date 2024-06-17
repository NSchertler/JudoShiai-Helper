using Avalonia.Controls;
using Avalonia;
using Shiai_Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace Shiai_Helper.UI
{
    public class TournamentBasedUserControlBase<TViewModel> : UserControl where TViewModel : ITournamentBasedViewModel
    {
        public static readonly DirectProperty<TournamentBasedUserControlBase<TViewModel>, Tournament?> TournamentProperty =
                AvaloniaProperty.RegisterDirect<TournamentBasedUserControlBase<TViewModel>, Tournament?>(
                    nameof(Tournament),
                    o => o.Tournament,
                    (o, v) => o.Tournament = v);

        public Tournament? Tournament
        {
            get => vm.Tournament;
            set => vm.Tournament = value;
        }

        protected TViewModel vm;

        public TournamentBasedUserControlBase()
        {
            this.AttachedToVisualTree += (s, e) => vm.Window = this.FindAncestorOfType<Window>();
        }
    }

    public interface ITournamentBasedViewModel
    {
        public Tournament? Tournament { get; set; }

        public Window Window { set; }
    }
}
