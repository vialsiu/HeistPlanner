using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    //---the roll is passed into Run so these tests dont depend on Random
    //-----full crew + no night checked = exactly 63% (same numbers as SuccessCalculatorTests)
    public class HeistSimulatorTests
    {
        private HeistPlan plan;
        private DateTime ranAt;

        [SetUp]
        public void Setup()
        {
            plan = new HeistPlan();
            plan.Hire(new Hacker("id-1", "Ana Ruiz", "Spain", 30, "", 80));
            plan.Hire(new Safecracker("id-2", "Tom Byrne", "Ireland", 45, "", 90));
            plan.Hire(new Lookout("id-3", "Mia Klein", "Germany", 25, "", 80));
            plan.Hire(new Driver("id-4", "Leo Rossi", "Italy", 38, "", 50));
            plan.Hire(new InsideMan("id-5", "Sam Park", "Canada", 52, "", 70));
            plan.AddTarget(new Artwork(1, "Water Lilies", "Claude Monet"));

            ranAt = new DateTime(2026, 10, 19, 20, 0, 0);
        }

        //---Run, boundary testing on the roll, at the chance gets away but one over gets caught
        [Test]
        public void Run_RollEqualsChance_GetsAway()
        {
            HeistResult result = HeistSimulator.Run(plan, 63, ranAt);
            Assert.That(result.Success, Is.True);
            Assert.That(result.TakeHome, Is.EqualTo(result.LootValue - result.CrewCut));
        }

        [Test]
        public void Run_RollOneOverChance_CaughtWithNoLoot()
        {
            HeistResult result = HeistSimulator.Run(plan, 64, ranAt);
            Assert.That(result.Success, Is.False);
            Assert.That(result.TakeHome, Is.EqualTo(0m));
        }

        //--loot maths, id 1 is 250,000 + 1 x 50,000 = 300,000 and 45% of 1,000,000 is 450,000
        [Test]
        public void LootMaths_EstimateAndCrewCut_ComeOutRight()
        {
            Assert.That(HeistSimulator.EstimateValue(new Artwork(1, "Water Lilies", "Claude Monet")), Is.EqualTo(300000m));
            Assert.That(HeistSimulator.GetCrewCut(1000000m, 45m), Is.EqualTo(450000m));
        }

        //---timeline, no driver + caught means the whole crew gets caught at the very end
        [Test]
        public void BuildTimeline_CaughtWithNoDriver_EndsWithWholeCrewCaught()
        {
            plan.Fire(CrewRole.Driver);
            List<TimelineEvent> timeline = HeistSimulator.BuildTimeline(plan, ranAt, false);
            Assert.That(timeline[timeline.Count - 1].Text, Is.EqualTo("The whole crew is caught."));
        }
    }
}
