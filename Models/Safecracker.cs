namespace HeistPlanner.Models
{
    public class Safecracker : CrewMember
    {
        public Safecracker() : base()
        {
        }

        public Safecracker(string id, string name, string country, int age, string photoUrl, int skill)
            : base(id, name, country, age, photoUrl, skill)
        {
        }

        public override CrewRole Role
        {
            get { return CrewRole.Safecracker; }
        }

        public override string Perk
        {
            get { return "Opens the display cases quietly and quickly."; }
        }

        public override double GetSuccessBonus()
        {
            return Skill / 9.0;
        }

        public override decimal GetCutPercentage()
        {
            return 15m;
        }
    }
}