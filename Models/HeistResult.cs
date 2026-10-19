namespace HeistPlanner.Models
{
    //---everything about one run of the heist
    public class HeistResult
    {
        private DateTime ranAt;
        private DateTime goTime;
        private double chance;
        private int roll;
        private bool success;
        private List<string> targetTitles = new List<string>();
        private List<TimelineEvent> timeline = new List<TimelineEvent>();
        private decimal lootValue;
        private decimal crewCut;
        private decimal takeHome;

        public DateTime RanAt
        {
            get { return ranAt; }
            set { ranAt = value; }
        }

        public DateTime GoTime
        {
            get { return goTime; }
            set { goTime = value; }
        }

        public double Chance
        {
            get { return chance; }
            set { chance = value; }
        }

        public int Roll
        {
            get { return roll; }
            set { roll = value; }
        }

        public bool Success
        {
            get { return success; }
            set { success = value; }
        }

        public List<string> TargetTitles
        {
            get { return targetTitles; }
            set { targetTitles = value; }
        }

        public List<TimelineEvent> Timeline
        {
            get { return timeline; }
            set { timeline = value; }
        }

        public decimal LootValue
        {
            get { return lootValue; }
            set { lootValue = value; }
        }

        public decimal CrewCut
        {
            get { return crewCut; }
            set { crewCut = value; }
        }

        public decimal TakeHome
        {
            get { return takeHome; }
            set { takeHome = value; }
        }

        public HeistResult()
        {
            ranAt = DateTime.Now;
            goTime = DateTime.Now;
        }

        public override string ToString()
        {
            string outcome = success ? "Clean getaway" : "Busted";
            return $"{outcome}: rolled {roll} against {chance}%, took home {takeHome:C0}";
        }
    }
}