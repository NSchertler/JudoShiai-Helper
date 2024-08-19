using Avalonia.Controls;
using Avalonia;
using Shiai_Helper.Models;

namespace Shiai_Helper.UI
{
    public abstract class TournamentBasedUserControlBase : UserControl
    {
        

        public abstract Tournament? Tournament { get; set; }
    }

    public class TournamentBasedUserControlBase<TViewModel>
        : TournamentBasedUserControlBase 
        where TViewModel : ITournamentBasedViewModel
    {
        public static readonly DirectProperty<TournamentBasedUserControlBase<TViewModel>, Tournament?> TournamentProperty =
                AvaloniaProperty.RegisterDirect<TournamentBasedUserControlBase<TViewModel>, Tournament?>(
                    nameof(Tournament),
                    o => o.Tournament,
                    (o, v) => o.Tournament = v);

        public override Tournament? Tournament
        {
            get => vm?.Tournament;
            set
            {
                if(vm != null)
                    vm.Tournament = value;
            }
        }

        protected TViewModel? vm;

        public TournamentBasedUserControlBase()
        {
            this.AttachedToVisualTree += (s, e) =>
            {
                if (vm != null)
                    vm.TopLevel = TopLevel.GetTopLevel(this);
            };
        }
    }

    public interface ITournamentBasedViewModel
    {
        public Tournament? Tournament { get; set; }

        public TopLevel? TopLevel { set; }
    }
}
