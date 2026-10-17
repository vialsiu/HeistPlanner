namespace HeistPlanner.Models
{
    public enum CrewRole
    {
        Hacker,
        Safecracker,
        Lookout,
        Driver,
        InsideMan
    }

    //---abstract cause you cant hire a "crew member", only a hacker, driver etc
    public abstract class CrewMember : IPayable
    {
        private string id;
        private string name;
        private string country;
        private int age;
        private string photoUrl;
        private int skill;      //---1 to 100

        public string Id
        {
            get { return id; }
            //---the id should never change so no need to set
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public string PhotoUrl
        {
            get { return photoUrl; }
            set { photoUrl = value; }
        }

        public int Skill
        {
            get { return skill; }
            set
            {
                //---keeps skill between 1 and 100 no matter what number comes in
                if (value < 1)
                {
                    skill = 1;
                }
                else if (value > 100)
                {
                    skill = 100;
                }
                else
                {
                    skill = value;
                }
            }
        }

        //----abstract so every role HAS to say what it is and what its perk is
        public abstract CrewRole Role { get; }
        public abstract string Perk { get; }

        public CrewMember()
        {
            //----ref Guid.NewGuid makes a unique id every time  https://learn.microsoft.com/en-us/dotnet/api/system.guid.newguid
            id = Guid.NewGuid().ToString();
            name = "unknown";
            country = "unknown";
            age = 0;
            photoUrl = "";
            skill = 50;
        }

        public CrewMember(string id, string name, string country, int age, string photoUrl, int skill)
        {
            this.id = id;
            this.name = name;
            this.country = country;
            this.age = age;
            this.photoUrl = photoUrl;
            Skill = skill;      //---using the property not the field so the 1-100 check runs
        }

        //----ref virtual = gives a default that subclasses CAN override https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/virtual
        //--- im trying to figure out how many % this person adds to the chance of success
        public virtual double GetSuccessBonus()
        {
            return skill / 10.0;
        }

        //---default cut of the loot, some roles take more or less
        //---decimal cause it gets multiplied with the loot money later
        public virtual decimal GetCutPercentage()
        {
            return 10m;
        }
        public static string GetRoleName(CrewRole role)
        {
            switch (role)
            {
                case CrewRole.Hacker:
                    return "Hacker";
                case CrewRole.Safecracker:
                    return "Safecracker";
                case CrewRole.Lookout:
                    return "Lookout";
                case CrewRole.Driver:
                    return "Driver";
                case CrewRole.InsideMan:
                    return "Inside Man";
                default:
                    return "Crew";
            }
        }

        public override string ToString()
        {
            return $"{GetRoleName(Role)} - {name}, country: {country}, age: {age}, skill: {skill}";
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is CrewMember))
            {
                return false;
            }

            var other = (CrewMember)obj;
            return other.id == id;
        }

        //----ref same null-safe == as in Artwork, Microsoft guidelines for overloading == https://learn.microsoft.com/en-us/previous-versions/ms173147(v=vs.90)
        public static bool operator ==(CrewMember? left, CrewMember? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(CrewMember? left, CrewMember? right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}