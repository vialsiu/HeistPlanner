using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    //---tests for the crew part of HeistPlan
    public class HeistPlanCrewTests
    {
        //Fields (private, camelCase)
        private HeistPlan plan;
        private Hacker ana;
        private Hacker tom;
        private InsideMan sam;
        private Driver leo;

        [SetUp]
        public void Setup()
        {
            plan = new HeistPlan();
            ana = new Hacker("id-ana", "Ana Ruiz", "Spain", 30, "", 80);
            tom = new Hacker("id-tom", "Tom Byrne", "Ireland", 45, "", 70);
            sam = new InsideMan("id-sam", "Sam Park", "Canada", 52, "", 60);
            leo = new Driver("id-leo", "Leo Rossi", "Italy", 38, "", 50);
        }

        //---Hire

        [Test]
        public void Hire_SameRoleTwice_ReplacesFirstPerson()
        {
            plan.Hire(ana);
            plan.Hire(tom);
            Assert.That(plan.GetCrewMember(CrewRole.Hacker), Is.EqualTo(tom));
        }

        //---GetTotalCutPercentage

        [Test]
        public void GetTotalCutPercentage_AddsUpEachRolesCut()
        {
            plan.Hire(ana);     //---hacker 15
            plan.Hire(sam);     //---inside man 20
            plan.Hire(leo);     //---driver 10 (default)
            Assert.That(plan.GetTotalCutPercentage(), Is.EqualTo(45m));
        }
    }
}