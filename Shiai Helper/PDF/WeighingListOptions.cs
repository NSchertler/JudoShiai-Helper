using MigraDoc.DocumentObjectModel;

namespace Shiai_Helper.PDF
{
    public class WeighingListOptions
    {
        public PageFormat PageSize { get; set; } = PageFormat.A5;

        public Orientation Orientation { get; set; } = Orientation.Landscape;

        public PageFormat PageSizeOverview { get; set; } = PageFormat.A4;
        public Orientation OrientationOverview { get; set; } = Orientation.Portrait;

        public bool UseExactWeight { get; set; } = false;
        public bool AddCheckColumn { get; set; } = false;
    }
}
