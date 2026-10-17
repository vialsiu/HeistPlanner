namespace HeistPlanner.Models
{
    public class Hacker : CrewMember
    {
        public Hacker() : base()
        {
        }

        public Hacker(string id, string name, string country, int age, string photoUrl, int skill)
            : base(id, name, country, age, photoUrl, skill)
        {
        }

        public override CrewRole Role
        {
            get { return CrewRole.Hacker; }
        }

        public override string Perk
        {
            get { return "Loops the camera feed so the guards see an empty gallery."; }
        }

        //---the hacker matters most on a museum job so their skill counts extra
        public override double GetSuccessBonus()
        {
            return Skill / 8.0;
        }

        public override decimal GetCutPercentage()
        {
            return 15m;
        }
    }
}