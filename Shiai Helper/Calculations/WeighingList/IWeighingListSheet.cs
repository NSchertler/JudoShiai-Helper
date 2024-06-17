using Shiai_Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.Calculations.WeighingList
{
    internal interface IWeighingListSheet
    {
        public string Title { get; }

        public IEnumerable<Competitor> Competitors { get; }

    }
}
