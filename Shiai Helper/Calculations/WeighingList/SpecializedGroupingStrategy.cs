using Shiai_Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.Calculations.WeighingList
{
    internal class SpecializedGroupingStrategy : IWeighingListGroupingStrategy
    {
        public IEnumerable<IWeighingListSheet> EnumerateSheets(Tournament tournament)
        {
            var weighingTimes = new Dictionary<string, int>()
            {
                {"U7", 1 },
                {"U9m", 1 },
                {"U9w", 1 },
                {"U11m", 1 },
                {"U11w", 1 }
            };

            Func<string, int> getWeighingTime = ageCat =>
            {
                if (weighingTimes.TryGetValue(ageCat, out int time))
                    return time;
                return 2;
            };

            var genderTerms = new Dictionary<Gender, string>()
            { { Gender.Male, "männlich"}, {Gender.Female, "weiblich"} };

            return tournament
                        .Competitors
                        .Select(c => c.Value)
                        .GroupBy(c => new { c.Gender, weighingTime = getWeighingTime(c.AgeCategory) })
                        .OrderBy(g => g.Key.weighingTime)
                        .Select(g => new SimpleWeighingListSheet($"Wiegen {g.Key.weighingTime}, {genderTerms[g.Key.Gender]}", g));
        }
    }
}
