namespace HeistPlanner.Models
{
    public class Lookout : CrewMember
    {
        public Lookout() : base()
        {
        }

        public Lookout(string id, string name, string country, int age, string photoUrl, int skill)
            : base(id, name, country, age, photoUrl, skill)
        {
        }

        public override CrewRole Role
        {
            get { return CrewRole.Lookout; }
        }

        public override string Perk
        {
            get { return "Spots the guard patrols before they get close."; }
        }

        //---uses the default success bonus from CrewMember, only the cut is different
        public override decimal GetCutPercentage()
        {
            return 8m;
        }
    }
}