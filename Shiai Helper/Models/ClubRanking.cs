﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.Models
{
    public class ClubRanking
    {
        public IList<ClubStatistics> ClubsSortedByRank { get; set; } = [];

        public ClubRankingOptions Options { get; set; } = new ClubRankingOptions();
    }
}
