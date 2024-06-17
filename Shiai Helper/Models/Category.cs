using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.Models
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int FoughtMatches { get; set; }
        public int UnfoughtMatches { get; set; }

        public CategoryState State
        {
            get
            {
                if (FoughtMatches == 0 && UnfoughtMatches == 0)
                {
                    if (winners.Any())
                        return CategoryState.Finished;
                    else
                        return CategoryState.NotDrawn;
                }
                if (UnfoughtMatches > 0)
                    return CategoryState.InProgress;

                if (winners.Any())
                    return CategoryState.Finished;
                else
                    return CategoryState.FinishedNoWinners;
            }
        }

        private List<(int competitorId, int place)> winners = new List<(int competitorId, int place)>();

        public IEnumerable<(int competitorId, int place)> Winners => winners;

        public Category(string name, int id)
        {
            Name = name;
            Id = id;
        }


        public void AddWinner(int competitorId, int place)
        {
            winners.Add((competitorId, place));
        }
    }
}
