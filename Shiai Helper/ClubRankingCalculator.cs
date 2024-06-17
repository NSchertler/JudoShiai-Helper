using MigraDoc.DocumentObjectModel.Tables;
using Shiai_Helper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper
{
    internal class ClubRankingCalculator
    {
        public ClubRanking CalculateClubRanking(Tournament tournament, ClubRankingOptions options)
        {
            var clubPoints = new Dictionary<string, ClubStatistics>();

            foreach (var club in tournament.Competitors.Select(c => c.Value.Club).Distinct())
                clubPoints.Add(club, new ClubStatistics() { Club = club });

            foreach (var cat in tournament.Categories.Values)
            {
                if (cat.State == CategoryState.Finished)
                {
                    foreach (var (winner, place) in cat.Winners)
                    {
                        var competitor = tournament.Competitors[winner];
                        clubPoints[competitor.Club].AddWinner(place);
                    }
                }
            }

            foreach (var club in clubPoints.Values)
            {
                club.TotalPoints = 0;
                foreach (var (place, count) in club.GetWinnerCounts())
                {
                    if (options.PointsPerPlace.TryGetValue(place, out int pointsForThisPlace))
                        club.TotalPoints += count * pointsForThisPlace;
                }
            }


            var ranking = new ClubRanking()
            {
                ClubsSortedByRank = clubPoints.Values.OrderByDescending(c => c.TotalPoints).ThenBy(c => c.Club).ToList(),
                Options = options.Clone()
            };

            int actualRank = 0;
            int countingRank = 0;
            int beforePoints = -1;
            foreach (var club in ranking.ClubsSortedByRank)
            {
                ++countingRank;
                if (club.TotalPoints != beforePoints)
                    actualRank = countingRank;
                beforePoints = club.TotalPoints;
                club.Rank = actualRank;               
            }

            return ranking;
        }
    }
}
