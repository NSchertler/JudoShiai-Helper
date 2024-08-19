using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.Models
{
    public class ClubStatistics
    {
        public int Rank { get; set; } = 0;

        public string Club { get; set; } = "";

        List<int> winnersByPlace = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 };

        public int TotalPoints { get; set; }

        public void AddWinner(int place)
        {
            winnersByPlace[place - 1]++;
        }

        public IReadOnlyList<int> WinnersByPlace { get; }

        public IEnumerable<(int place, int count)> GetWinnerCounts()
        {
            for (int i = 0; i < winnersByPlace.Count; ++i)
            {
                yield return (i + 1, winnersByPlace[i]);
            }
        }

        public ClubStatistics()
        {
            WinnersByPlace = new ReadOnlyCollection<int>(winnersByPlace);
        }
    }
}
