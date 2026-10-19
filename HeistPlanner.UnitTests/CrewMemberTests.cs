using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    public class CrewMemberTests
    {
        private Hacker hacker;

        //---skills picked so the bonus maths comes out to round numbers
        [SetUp]
        public void Setup()
        {
            hacker = new Hacker("id-1", "Ana Ruiz", "Spain", 30, "", 80);
        }

        //---boundary testing on the Skill setter, it should always stay between 1 and 100
        [Test]
        public void Skill_Zero_BecomesOne()
        {
            Driver testDriver = new Driver("id-9", "Test", "Test", 20, "", 0);
            Assert.That(testDriver.Skill, Is.EqualTo(1));
        }

        [Test]
        public void Skill_HundredAndOne_BecomesHundred()
        {
            Driver testDriver = new Driver("id-9", "Test", "Test", 20, "", 101);
            Assert.That(testDriver.Skill, Is.EqualTo(100));
        }

        //---success bonus, overridden in some roles, default (skill / 10) in others
        [Test]
        public void GetSuccessBonus_Hacker_UsesOwnFormula()
        {
            Assert.That(hacker.GetSuccessBonus(), Is.EqualTo(10.0));      //---80 / 8
        }

        //---polymorphism, even stored as the parent type the override still runs

        [Test]
        public void GetCutPercentage_StoredAsIPayable_StillUsesOverride()
        {
            IPayable payable = hacker;
            Assert.That(payable.GetCutPercentage(), Is.EqualTo(15m));
        }

    }
}