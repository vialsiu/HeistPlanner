using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    //--the bonuses are picked so the maths is easy to check by hand:
    //----hacker 80/8=10, safecracker 90/9=10, lookout 80/10=8, driver 50/10=5, inside man 70/7=10, so a full crew adds 43
    public class SuccessCalculatorTests
    {
        //Fields (private, camelCase)
        private HeistPlan plan;
        private NightReport night;

        [SetUp]
        public void Setup()
        {
            plan = new HeistPlan();

            //---dark from 19:57 on the 30th to 06:23 on the 31st
            night = new NightReport(
                new DateTime(2026, 10, 30),
                new DateTime(2026, 10, 30, 18, 23, 0),
                new DateTime(2026, 10, 30, 19, 57, 0),
                new DateTime(2026, 10, 31, 6, 23, 0),
                new DateTime(2026, 10, 31, 7, 57, 0),
                "Waning Crescent",
                20);
        }

        //---helpers so each test only sets up what it needs
        private void HireFullCrew()
        {
            plan.Hire(new Hacker("id-1", "Ana Ruiz", "Spain", 30, "", 80));
            plan.Hire(new Safecracker("id-2", "Tom Byrne", "Ireland", 45, "", 90));
            plan.Hire(new Lookout("id-3", "Mia Klein", "Germany", 25, "", 80));
            plan.Hire(new Driver("id-4", "Leo Rossi", "Italy", 38, "", 50));
            plan.Hire(new InsideMan("id-5", "Sam Park", "Canada", 52, "", 70));
        }

        private WeatherReport MakeWeather(int code, int cloud)
        {
            return new WeatherReport(new DateTime(2026, 10, 31, 1, 0, 0), 10, 0, cloud, 24000, 15, code);
        }

        //---GetChance, both caps and one exact answer
        [Test]
        public void GetChance_EmptyPlan_IsRaisedToTheMinimum()
        {
            //---30 - 40 - 10 = -20, raised to 5
            double chance = SuccessCalculator.GetChance(SuccessCalculator.GetFactors(plan));
            Assert.That(chance, Is.EqualTo(SuccessCalculator.MIN_CHANCE));
        }

        [Test]
        public void GetChance_FullCrewNightNotChecked_IsExactlySixtyThree()
        {
            //---30 + 43 - 10 = 63
            HireFullCrew();
            double chance = SuccessCalculator.GetChance(SuccessCalculator.GetFactors(plan));
            Assert.That(chance, Is.EqualTo(63));
        }

        [Test]
        public void GetChance_FullCrewPerfectNight_IsCappedAtTheMaximum()
        {
            //---30 + 43 + dark 10 + dark moon 4 + fog 10 = 97, capped to 95
            HireFullCrew();
            plan.SetNight(night, MakeWeather(45, 100), 1);
            double chance = SuccessCalculator.GetChance(SuccessCalculator.GetFactors(plan));
            Assert.That(chance, Is.EqualTo(SuccessCalculator.MAX_CHANCE));
        }

        //---sorting with Factor.CompareTo

        [Test]
        public void Sort_BigPenaltyGoesBeforeSmallBonus()
        {
            List<Factor> factors = new List<Factor>();
            factors.Add(new Factor("Small bonus", 4));
            factors.Add(new Factor("Big penalty", -15));
            factors.Sort();
            Assert.That(factors[0].Label, Is.EqualTo("Big penalty"));
        }

    }
}