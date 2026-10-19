namespace HeistPlanner.Models
{
    //-----runs the heist
    //----rolls the dice against the chance, writes the story of the night and splits the loot
    public class HeistSimulator
    {
        //---the museum API has no prices so every piece gets a made up estimate between 250,000 and 1,200,000
        public static readonly decimal BASE_PIECE_VALUE = 250000m;
        public static readonly decimal VALUE_STEP = 50000m;

        private static Random generator = new Random();     //---same Random as the week 1 guessing game

        //-----1 to 100, the page rolls this and passes it into Run so the tests can pass in their own number
        public static int RollDice()
        {
            return generator.Next(1, 101);
        }

        //---made up value, the same artwork always gets the same number
        public static decimal EstimateValue(Artwork artwork)
        {
            return BASE_PIECE_VALUE + (artwork.Id % 20) * VALUE_STEP;
        }

        public static decimal GetLootValue(List<Artwork> targets)
        {
            decimal total = 0m;
            foreach (Artwork artwork in targets)
            {
                total = total + EstimateValue(artwork);
            }
            return total;
        }

        //---the museum is in the US so the made up values are in dollars, N0 = commas and no decimals
        //----ref standard numeric format strings https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        public static string FormatMoney(decimal amount)
        {
            return "$" + amount.ToString("N0");
        }

        //---e.g. 45% of 1,000,000 = 450,000
        public static decimal GetCrewCut(decimal lootValue, decimal cutPercentage)
        {
            return lootValue * cutPercentage / 100m;
        }

        //----roll at or under the chance = success, e.g. chance 63 and roll 41 gets away
        public static HeistResult Run(HeistPlan plan, int roll, DateTime ranAt)
        {
            double chance = SuccessCalculator.GetChance(SuccessCalculator.GetFactors(plan));
            bool success = roll <= chance;
            DateTime goTime = GetGoTimeOrDefault(plan, ranAt);

            HeistResult result = new HeistResult();
            result.RanAt = ranAt;
            result.GoTime = goTime;
            result.Chance = chance;
            result.Roll = roll;
            result.Success = success;
            result.Timeline = BuildTimeline(plan, goTime, success);

            foreach (Artwork artwork in plan.Targets)
            {
                result.TargetTitles.Add(artwork.Title);
            }

            //----no loot if they got caught
            if (success)
            {
                result.LootValue = GetLootValue(plan.Targets);
                result.CrewCut = GetCrewCut(result.LootValue, plan.GetTotalCutPercentage());
                result.TakeHome = result.LootValue - result.CrewCut;
            }

            return result;
        }

        //-----if no night was checked the heist happens at 1am the next day
        private static DateTime GetGoTimeOrDefault(HeistPlan plan, DateTime ranAt)
        {
            if (plan.GoTime != null)
            {
                return plan.GoTime.Value;
            }
            return ranAt.Date.AddDays(1).AddHours(1);
        }

        //----the story of the night, each step depends on who was hired
        public static List<TimelineEvent> BuildTimeline(HeistPlan plan, DateTime goTime, bool success)
        {
            List<TimelineEvent> timeline = new List<TimelineEvent>();
            timeline.Add(new TimelineEvent(goTime, $"The crew meets two streets from the {Museum.NAME}.", false));

            CrewMember? insideMan = plan.GetCrewMember(CrewRole.InsideMan);
            if (insideMan != null)
            {
                timeline.Add(new TimelineEvent(goTime.AddMinutes(3), $"{insideMan.Name} leaves the staff door unlocked.", false));
            }
            else
            {
                timeline.Add(new TimelineEvent(goTime.AddMinutes(3), "No inside man, so the crew picks the lock on the loading bay.", true));
            }

            CrewMember? hacker = plan.GetCrewMember(CrewRole.Hacker);
            if (hacker != null)
            {
                timeline.Add(new TimelineEvent(goTime.AddMinutes(6), $"{hacker.Name} loops the camera feed.", false));
            }
            else
            {
                timeline.Add(new TimelineEvent(goTime.AddMinutes(6), "Nobody is watching the cameras.", true));
            }

            CrewMember? lookout = plan.GetCrewMember(CrewRole.Lookout);
            if (lookout != null)
            {
                timeline.Add(new TimelineEvent(goTime.AddMinutes(12), $"{lookout.Name} spots the guard patrol and signals the all clear.", false));
            }
            else
            {
                timeline.Add(new TimelineEvent(goTime.AddMinutes(12), "No lookout, so nobody sees the patrol coming.", true));
            }

            CrewMember? safecracker = plan.GetCrewMember(CrewRole.Safecracker);
            if (safecracker != null)
            {
                timeline.Add(new TimelineEvent(goTime.AddMinutes(18), $"{safecracker.Name} opens the display cases without a sound.", false));
            }
            else
            {
                timeline.Add(new TimelineEvent(goTime.AddMinutes(18), "The crew forces the cases open and sets off a sensor.", true));
            }

            //-----one line for every piece, 2 minutes apart
            int minute = 24;
            foreach (Artwork artwork in plan.Targets)
            {
                timeline.Add(new TimelineEvent(goTime.AddMinutes(minute), $"{artwork.Title} comes off the wall.", false));
                minute = minute + 2;
            }

            AddEnding(plan, timeline, goTime.AddMinutes(minute + 6), success);

            timeline.Sort();    //-----uses TimelineEvent.CompareTo so everything is in time order
            return timeline;
        }

        private static void AddEnding(HeistPlan plan, List<TimelineEvent> timeline, DateTime time, bool success)
        {
            CrewMember? driver = plan.GetCrewMember(CrewRole.Driver);

            if (success)
            {
                if (driver != null)
                {
                    timeline.Add(new TimelineEvent(time, $"{driver.Name} drives the crew away. Clean getaway.", false));
                }
                else
                {
                    timeline.Add(new TimelineEvent(time, "No driver, so the crew escapes on foot. Somehow it works.", false));
                }
                return;
            }

            timeline.Add(new TimelineEvent(time, "Alarms go off. Police cars pull up outside the museum.", true));
            if (driver != null)
            {
                timeline.Add(new TimelineEvent(time.AddMinutes(2), $"{driver.Name} gets away alone. The rest of the crew is caught.", true));
            }
            else
            {
                timeline.Add(new TimelineEvent(time.AddMinutes(2), "The whole crew is caught.", true));
            }
        }
    }
}