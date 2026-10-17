namespace HeistPlanner.Models
{
    public class Driver : CrewMember
    {
        public Driver() : base()
        {
        }

        public Driver(string id, string name, string country, int age, string photoUrl, int skill)
            : base(id, name, country, age, photoUrl, skill)
        {
        }

        public override CrewRole Role
        {
            get { return CrewRole.Driver; }
        }

        public override string Perk
        {
            get { return "Gets the crew and the loot away fast."; }
        }

        //---no method overrides, the driver uses the default bonus and cut from CrewMember
        //---thats the point of virtual here, you only override if you need something different
    }
}