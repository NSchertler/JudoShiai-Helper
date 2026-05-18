using Shiai_Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shiai_Helper.Calculations.WeighingList
{
    internal class SimpleWeighingListGroupingStrategy : IWeighingListGroupingStrategy
    {
        public bool SeparateByAgeCategory { get; set; } = true;

        public bool SeparateByGender { get; set;} = true;

        public IEnumerable<IWeighingListSheet> EnumerateSheets(Tournament tournament)
        {
            return tournament
                        .Competitors
                        .Select(c => c.Value)
                        .GroupBy(GetSheetForCompetitor)
                        .OrderBy(g => g.Key)
                        .Select(g => new SimpleWeighingListSheet(g.Key.GetTitle(), g));
        }

        public SimpleSheetDescriptor GetSheetForCompetitor(Competitor competitor)
        {
            var descriptor = new SimpleSheetDescriptor(competitor.Club);

            if (SeparateByAgeCategory && SeparateByGender)
                descriptor.AddSeparatingValue(competitor.AgeCategory);
            else if(SeparateByGender || SeparateByAgeCategory)
            {
                var gender = "männlich";
                var ageCategory = competitor.AgeCategory;
                var hasFemaleSuffix = competitor.AgeCategory.EndsWith("w");
                var hasMaleSuffix = competitor.AgeCategory.EndsWith("m");
                if (hasFemaleSuffix || hasMaleSuffix)
                {
                    ageCategory = competitor.AgeCategory.Substring(0, competitor.AgeCategory.Length - 1);
                    gender = hasMaleSuffix ? "männlich" : "weiblich";
                }

                if(SeparateByAgeCategory)
                    descriptor.AddSeparatingValue(ageCategory);

                if (SeparateByGender)
                    descriptor.AddSeparatingValue(gender);
            }

            return descriptor;
        }

        public sealed record SimpleSheetDescriptor : IComparable<SimpleSheetDescriptor>
        {
            private string club;

            private List<string> additionalValues = [];

            public SimpleSheetDescriptor(string club)
            {
                this.club = club;
            }

            public void AddSeparatingValue(string value)
            {
                additionalValues.Add(value);
            }

            

            public int CompareTo(SimpleSheetDescriptor? other)
            {
                if (this == other) return 0;

                if (other is null) return 1;

                if (club != other.club)
                    return club.CompareTo(other.club);

                for(int i = 0; i <  additionalValues.Count; i++)
                {
                    var thisValue = additionalValues[i];
                    var otherValue = other.additionalValues[i];
                    var comparisonResult = thisValue.CompareTo(otherValue);
                    if (comparisonResult != 0)
                        return comparisonResult;
                }

                return 0;
            }

            public string GetTitle() => string.Join(", ", [club, ..additionalValues]);

            public bool Equals(SimpleSheetDescriptor? other)
            {
                if (ReferenceEquals(this, other)) return true;
                if (ReferenceEquals(null, other)) return false;

                if(club == other.club) return true;
                for (int i = 0; i < additionalValues.Count; i++)
                {
                    var thisValue = additionalValues[i];
                    var otherValue = other.additionalValues[i];
                    if (thisValue != otherValue)
                        return false;
                }

                return false;
            }

            public override int GetHashCode()
            {
                var hash = club.GetHashCode();
                foreach (var value in additionalValues)
                    hash ^= value.GetHashCode();
                return hash;
            }
        }
    }
}
