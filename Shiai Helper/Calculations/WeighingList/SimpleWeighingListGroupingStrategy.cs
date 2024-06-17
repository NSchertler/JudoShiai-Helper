using Shiai_Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.Calculations.WeighingList
{
    internal class SimpleWeighingListGroupingStrategy : IWeighingListGroupingStrategy
    {
        public IEnumerable<IWeighingListSheet> EnumerateSheets(Tournament tournament)
        {
            return tournament
                        .Competitors
                        .Select(c => c.Value)
                        .GroupBy(c => new { c.Club, c.AgeCategory })
                        .OrderBy(g => g.Key.Club).ThenBy(g => g.Key.AgeCategory)
                        .Select(g => new SimpleWeighingListSheet($"{g.Key.Club}, {g.Key.AgeCategory}", g));
        }
    }
}
