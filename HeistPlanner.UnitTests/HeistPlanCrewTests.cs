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

        [Test]
        public void NewPlan_HasNoCrew()
        {
            Assert.That(plan.Crew.Count, Is.EqualTo(0));
        }

        //---Hire
        [Test]
        public void Hire_AddsToCrew()
        {
            plan.Hire(ana);
            Assert.That(plan.Crew.Count, Is.EqualTo(1));
        }

        [Test]
        public void Hire_CanBeFoundByRole()
        {
            plan.Hire(ana);
            Assert.That(plan.GetCrewMember(CrewRole.Hacker), Is.EqualTo(ana));
        }

        [Test]
        public void Hire_SameRoleTwice_ReplacesFirstPerson()
        {
            plan.Hire(ana);
            plan.Hire(tom);
            Assert.That(plan.GetCrewMember(CrewRole.Hacker), Is.EqualTo(tom));
        }

        [Test]
        public void Hire_SameRoleTwice_StillOnlyOneInThatRole()
        {
            plan.Hire(ana);
            plan.Hire(tom);
            Assert.That(plan.Crew.Count, Is.EqualTo(1));
        }

        [Test]
        public void Hire_DifferentRoles_KeepsBoth()
        {
            plan.Hire(ana);
            plan.Hire(leo);
            Assert.That(plan.Crew.Count, Is.EqualTo(2));
        }

        //---Fire , GetCrewMember
        [Test]
        public void Fire_RemovesThatRole()
        {
            plan.Hire(ana);
            plan.Fire(CrewRole.Hacker);
            Assert.That(plan.GetCrewMember(CrewRole.Hacker), Is.Null);
        }

        [Test]
        public void Fire_EmptyRole_DoesNothing()
        {
            plan.Hire(ana);
            plan.Fire(CrewRole.Driver);
            Assert.That(plan.Crew.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetCrewMember_NobodyHired_ReturnsNull()
        {
            Assert.That(plan.GetCrewMember(CrewRole.Lookout), Is.Null);
        }

        //---GetTotalCutPercentage
        [Test]
        public void GetTotalCutPercentage_NoCrew_IsZero()
        {
            Assert.That(plan.GetTotalCutPercentage(), Is.EqualTo(0m));
        }

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