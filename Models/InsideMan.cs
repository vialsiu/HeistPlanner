namespace HeistPlanner.Models
{
    public class InsideMan : CrewMember
    {
        public InsideMan() : base()
        {
        }

        public InsideMan(string id, string name, string country, int age, string photoUrl, int skill)
            : base(id, name, country, age, photoUrl, skill)
        {
        }

        public override CrewRole Role
        {
            get { return CrewRole.InsideMan; }
        }

        public override string Perk
        {
            get { return "Works at the museum and knows the guard rota."; }
        }

        //---knowing the inside is worth the most, they want the biggest cut too lol
        public override double GetSuccessBonus()
        {
            return Skill / 7.0;
        }

        public override decimal GetCutPercentage()
        {
            return 20m;
        }
    }
}