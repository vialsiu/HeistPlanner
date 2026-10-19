namespace HeistPlanner.Models
{
    //---one thing that changes the chance of success, e.g. Ana Ruiz (Hacker) +10
    public class Factor : IComparable<Factor>
    {
        private string label;
        private double effect;      //---percentage points, can be negative

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        public double Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        public bool IsPositive
        {
            get { return effect >= 0; }
        }

        public Factor()
        {
            label = "unknown";
            effect = 0;
        }

        public Factor(string label, double effect)
        {
            this.label = label;
            this.effect = effect;
        }

        public override string ToString()
        {
            //---+10 or -8, the + is added by hand because positive numbers dont show one
            string sign = effect >= 0 ? "+" : "";
            return $"{label} {sign}{effect}";
        }

        //---biggest effect first, good or bad, so the factors that matter most are at the top
        public int CompareTo(Factor? other)
        {
            if (other is null) return -1;
            return Math.Abs(other.effect).CompareTo(Math.Abs(effect));
        }
    }
}