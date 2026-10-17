namespace HeistPlanner.Models
{
    public class HeistPlan
    {
        public static readonly int MAX_TARGETS = 3;
        private List<Artwork> targets = new List<Artwork>();

        public List<Artwork> Targets
        {
            get { return targets; }
        }

        public HeistPlan()
        {
        }

        //------returns null if the artwork was added
        public string? AddTarget(Artwork artwork)
        {
            //----contains() uses overridden equals so the same artwork can't be added twice
            if (targets.Contains(artwork))
            {
                return $"{artwork.Title} is already on your target list.";
            }

            if (targets.Count >= MAX_TARGETS)
            {
                return $"Your crew can only carry {MAX_TARGETS} pieces. Remove one first.";
            }

            targets.Add(artwork);
            return null;
        }

        public void RemoveTarget(Artwork artwork)
        {
            //---remove() also uses our equals to find the right artwork
            targets.Remove(artwork);
        }

        public bool IsTarget(Artwork artwork)
        {
            return targets.Contains(artwork);
        }
    }
}