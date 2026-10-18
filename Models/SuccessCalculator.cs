namespace HeistPlanner.Models
{
    //---works out the chance of success from everything in the plan
    //---this is the aggregator: targets (Cleveland), crew (RandomUser), darkness (Sunrise-Sunset) and weather (Open-Meteo) all end up in one number
    public class SuccessCalculator
    {
        //--every number used in the formula is here so its easy to see and change
        public static readonly double BASE_CHANCE = 30;
        public static readonly double MIN_CHANCE = 5;
        public static readonly double MAX_CHANCE = 95;
        public static readonly double MISSING_ROLE_PENALTY = -8;
        public static readonly double DARK_BONUS = 10;
        public static readonly double NOT_DARK_PENALTY = -15;
        public static readonly double DARK_MOON_BONUS = 4;
        public static readonly double BRIGHT_MOON_PENALTY = -6;
        public static readonly double EXCELLENT_COVER_BONUS = 10;
        public static readonly double SOME_COVER_BONUS = 5;
        public static readonly double NO_COVER_PENALTY = -8;
        public static readonly double NIGHT_NOT_CHECKED_PENALTY = -10;
        public static readonly double EXTRA_PIECE_PENALTY = -4;

        //----every factor that changes the chance, biggest effect first
        public static List<Factor> GetFactors(HeistPlan plan)
        {
            List<Factor> factors = new List<Factor>();
            factors.Add(new Factor("Base chance", BASE_CHANCE));

            AddCrewFactors(plan, factors);
            AddNightFactors(plan, factors);
            AddLoadFactor(plan, factors);

            factors.Sort();     //---uses Factor.CompareTo
            return factors;
        }

        //--adds all the factors up and keeps the answer between 5% and 95%
        public static double GetChance(List<Factor> factors)
        {
            double total = 0;
            foreach (Factor factor in factors)
            {
                total = total + factor.Effect;
            }

            if (total < MIN_CHANCE)
            {
                total = MIN_CHANCE;
            }
            else if (total > MAX_CHANCE)
            {
                total = MAX_CHANCE;
            }

            return Math.Round(total, 1);
        }

        //-----each hired person adds their own bonus (Hacker, InsideMan etc override it), each empty role costs points
        private static void AddCrewFactors(HeistPlan plan, List<Factor> factors)
        {
            //-----ref Enum.GetValues to loop through all 5 roles - https://learn.microsoft.com/en-us/dotnet/api/system.enum.getvalues
            foreach (CrewRole role in Enum.GetValues<CrewRole>())
            {
                CrewMember? member = plan.GetCrewMember(role);
                string roleName = CrewMember.GetRoleName(role);

                if (member != null)
                {
                    factors.Add(new Factor($"{member.Name} ({roleName})", Math.Round(member.GetSuccessBonus(), 1)));
                }
                else
                {
                    factors.Add(new Factor($"No {roleName}", MISSING_ROLE_PENALTY));
                }
            }
        }

        //----darkness, moon and weather from The Night page
        private static void AddNightFactors(HeistPlan plan, List<Factor> factors)
        {
            if (plan.Night == null || plan.Weather == null || plan.GoTime == null)
            {
                factors.Add(new Factor("Night not checked", NIGHT_NOT_CHECKED_PENALTY));
                return;
            }

            if (plan.Night.IsDarkAt(plan.GoTime.Value))
            {
                factors.Add(new Factor("Full darkness at go time", DARK_BONUS));
            }
            else
            {
                factors.Add(new Factor("Not fully dark at go time", NOT_DARK_PENALTY));
            }

            if (plan.Night.IsBrightMoon())
            {
                factors.Add(new Factor("Bright moon", BRIGHT_MOON_PENALTY));
            }
            else
            {
                factors.Add(new Factor("Dark moon", DARK_MOON_BONUS));
            }

            WeatherCover cover = plan.Weather.GetCover();
            switch (cover)
            {
                case WeatherCover.Excellent:
                    factors.Add(new Factor($"{plan.Weather.GetDescription()} hides the crew", EXCELLENT_COVER_BONUS));
                    break;
                case WeatherCover.Good:
                    factors.Add(new Factor($"{plan.Weather.GetDescription()} gives some cover", SOME_COVER_BONUS));
                    break;
                default:
                    factors.Add(new Factor($"{plan.Weather.GetDescription()}, no cover", NO_COVER_PENALTY));
                    break;
            }
        }

        //----every piece after the first makes the getaway slower
        private static void AddLoadFactor(HeistPlan plan, List<Factor> factors)
        {
            int extraPieces = plan.Targets.Count - 1;
            if (extraPieces > 0)
            {
                factors.Add(new Factor($"Carrying {plan.Targets.Count} pieces", extraPieces * EXTRA_PIECE_PENALTY));
            }
        }
    }
}