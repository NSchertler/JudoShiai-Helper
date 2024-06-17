using Shiai_Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.Calculations.WeighingList
{
    internal class SimpleWeighingListSheet : IWeighingListSheet
    {
        private IGrouping<object, Competitor> g;

        public SimpleWeighingListSheet(string title, IGrouping<object, Competitor> g)
        {
            this.Title = title;
            this.g = g;
        }

        public string Title { get; }

        public IEnumerable<Competitor> Competitors => g.OrderBy(c => c.LastName).ThenBy(c => c.FirstName);
    }
}
