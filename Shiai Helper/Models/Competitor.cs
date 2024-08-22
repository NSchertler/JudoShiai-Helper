namespace Shiai_Helper.Models
{
    public class Competitor
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string Club { get; set; } = "";

        public string AgeCategory { get; set; } = "";

        public string WeightCategory { get; set; } = "";

        public double WeightKilograms { get; set; } = 0;

        public Gender Gender { get; set; } = Gender.Unknown;

        public string CoachId { get; set; } = "";
    }
}
