using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Shiai_Helper.Models
{
    public class Competitor
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Club { get; set; }

        public string AgeCategory { get; set; }

        public string WeightCategory { get; set; }

        public Gender Gender { get; set; }

        public string CoachId { get; set; }
    }
}
