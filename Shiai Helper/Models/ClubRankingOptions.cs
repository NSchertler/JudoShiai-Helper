using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.Models
{
    public class ClubRankingOptions
    {
        public Dictionary<int, int> PointsPerPlace { get; private set; } = new Dictionary<int, int>();

        public void SetAwardedPointsForPlace(int place, int points)
        {
            if (!PointsPerPlace.TryAdd(place, points))
                PointsPerPlace[place] = points;
        }

        public ClubRankingOptions Clone()
        {
            return new ClubRankingOptions { PointsPerPlace = new Dictionary<int, int>(PointsPerPlace) };
        }
    }
}
