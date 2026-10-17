namespace HeistPlanner.Models
{
    public class HeistPlan
    {
        public static readonly int MAX_TARGETS = 3;
        private List<Artwork> targets = new List<Artwork>();

        //----ref Dictionary, like a list but every item has a key and a key can only be in there once https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2
        //---the key is the role so theres only ever one person per role
        private Dictionary<CrewRole, CrewMember> crew = new Dictionary<CrewRole, CrewMember>();

        public List<Artwork> Targets
        {
            get { return targets; }
        }

        public Dictionary<CrewRole, CrewMember> Crew
        {
            get { return crew; }
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

        //---hiring someone into a role thats already taken replaces the old person
        public void Hire(CrewMember member)
        {
            crew[member.Role] = member;
        }

        public void Fire(CrewRole role)
        {
            crew.Remove(role);
        }

        //---gives back null if nobody has that role yet
        public CrewMember? GetCrewMember(CrewRole role)
        {
            if (crew.ContainsKey(role))
            {
                return crew[role];
            }
            return null;
        }

        //---adds up everyones cut, each role uses its own GetCutPercentage (overrides from the subclasses)
        public decimal GetTotalCutPercentage()
        {
            decimal total = 0m;
            foreach (CrewMember member in crew.Values)
            {
                total = total + member.GetCutPercentage();
            }
            return total;
        }
    }
}