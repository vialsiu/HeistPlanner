using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    public class CrewMemberTests
    {
        private Hacker hacker;
        private Safecracker safecracker;
        private Lookout lookout;
        private Driver driver;
        private InsideMan insideMan;

        //---skills picked so the bonus maths comes out to round numbers
        [SetUp]
        public void Setup()
        {
            hacker = new Hacker("id-1", "Ana Ruiz", "Spain", 30, "", 80);
            safecracker = new Safecracker("id-2", "Tom Byrne", "Ireland", 45, "", 90);
            lookout = new Lookout("id-3", "Mia Klein", "Germany", 25, "", 80);
            driver = new Driver("id-4", "Leo Rossi", "Italy", 38, "", 50);
            insideMan = new InsideMan("id-5", "Sam Park", "Canada", 52, "", 70);
        }

        //---boundary testing on the Skill setter, it should always stay between 1 and 100
        [Test]
        public void Skill_Zero_BecomesOne()
        {
            Driver testDriver = new Driver("id-9", "Test", "Test", 20, "", 0);
            Assert.That(testDriver.Skill, Is.EqualTo(1));
        }

        [Test]
        public void Skill_One_StaysOne()
        {
            Driver testDriver = new Driver("id-9", "Test", "Test", 20, "", 1);
            Assert.That(testDriver.Skill, Is.EqualTo(1));
        }

        [Test]
        public void Skill_Hundred_StaysHundred()
        {
            Driver testDriver = new Driver("id-9", "Test", "Test", 20, "", 100);
            Assert.That(testDriver.Skill, Is.EqualTo(100));
        }

        [Test]
        public void Skill_HundredAndOne_BecomesHundred()
        {
            Driver testDriver = new Driver("id-9", "Test", "Test", 20, "", 101);
            Assert.That(testDriver.Skill, Is.EqualTo(100));
        }

        [Test]
        public void Skill_SetThroughProperty_IsAlsoChecked()
        {
            hacker.Skill = 250;
            Assert.That(hacker.Skill, Is.EqualTo(100));
        }

        //---empty constructor
        [Test]
        public void EmptyConstructor_GivesDefaultSkill()
        {
            Driver newDriver = new Driver();
            Assert.That(newDriver.Skill, Is.EqualTo(50));
        }

        [Test]
        public void EmptyConstructor_GivesEveryoneAUniqueId()
        {
            Driver first = new Driver();
            Driver second = new Driver();
            Assert.That(first.Id, Is.Not.EqualTo(second.Id));
        }

        //---each subclass reports its own role
        [Test]
        public void Role_EachSubclass_ReturnsItsOwnRole()
        {
            Assert.That(hacker.Role, Is.EqualTo(CrewRole.Hacker));
            Assert.That(safecracker.Role, Is.EqualTo(CrewRole.Safecracker));
            Assert.That(lookout.Role, Is.EqualTo(CrewRole.Lookout));
            Assert.That(driver.Role, Is.EqualTo(CrewRole.Driver));
            Assert.That(insideMan.Role, Is.EqualTo(CrewRole.InsideMan));
        }

        //---success bonus, overridden in some roles, default (skill / 10) in others
        [Test]
        public void GetSuccessBonus_Hacker_UsesOwnFormula()
        {
            Assert.That(hacker.GetSuccessBonus(), Is.EqualTo(10.0));      //---80 / 8
        }

        [Test]
        public void GetSuccessBonus_Safecracker_UsesOwnFormula()
        {
            Assert.That(safecracker.GetSuccessBonus(), Is.EqualTo(10.0)); //---90 / 9
        }

        [Test]
        public void GetSuccessBonus_InsideMan_UsesOwnFormula()
        {
            Assert.That(insideMan.GetSuccessBonus(), Is.EqualTo(10.0));   //---70 / 7
        }

        [Test]
        public void GetSuccessBonus_Lookout_UsesDefault()
        {
            Assert.That(lookout.GetSuccessBonus(), Is.EqualTo(8.0));      //---80 / 10
        }

        [Test]
        public void GetSuccessBonus_Driver_UsesDefault()
        {
            Assert.That(driver.GetSuccessBonus(), Is.EqualTo(5.0));       //---50 / 10
        }

        //---cut of the loot
        [Test]
        public void GetCutPercentage_Driver_UsesDefaultTen()
        {
            Assert.That(driver.GetCutPercentage(), Is.EqualTo(10m));
        }

        [Test]
        public void GetCutPercentage_Lookout_TakesLess()
        {
            Assert.That(lookout.GetCutPercentage(), Is.EqualTo(8m));
        }

        [Test]
        public void GetCutPercentage_InsideMan_TakesMost()
        {
            Assert.That(insideMan.GetCutPercentage(), Is.EqualTo(20m));
        }

        //---polymorphism, even stored as the parent type the override still runs
        [Test]
        public void GetCutPercentage_StoredAsCrewMember_StillUsesOverride()
        {
            CrewMember member = insideMan;
            Assert.That(member.GetCutPercentage(), Is.EqualTo(20m));
        }

        [Test]
        public void GetCutPercentage_StoredAsIPayable_StillUsesOverride()
        {
            IPayable payable = hacker;
            Assert.That(payable.GetCutPercentage(), Is.EqualTo(15m));
        }

        [Test]
        public void Hacker_IsACrewMember()
        {
            Assert.That(hacker, Is.InstanceOf<CrewMember>());
            Assert.That(hacker, Is.TypeOf<Hacker>());
        }

        //---GetRoleName switch
        [Test]
        public void GetRoleName_InsideMan_HasASpace()
        {
            Assert.That(CrewMember.GetRoleName(CrewRole.InsideMan), Is.EqualTo("Inside Man"));
        }

        [Test]
        public void GetRoleName_Hacker_ReturnsHacker()
        {
            Assert.That(CrewMember.GetRoleName(CrewRole.Hacker), Is.EqualTo("Hacker"));
        }

        //---Equals, == and ToString
        [Test]
        public void Equals_SameId_ReturnsTrue()
        {
            Driver sameDriver = new Driver("id-4", "Different Name", "France", 60, "", 10);
            Assert.That(driver.Equals(sameDriver), Is.True);
        }

        [Test]
        public void Equals_DifferentId_ReturnsFalse()
        {
            Assert.That(hacker.Equals(driver), Is.False);
        }

        [Test]
        public void EqualsOperator_NullOnLeft_ReturnsFalseWithoutCrashing()
        {
            CrewMember? nobody = null;
            Assert.That(nobody == hacker, Is.False);
        }

        [Test]
        public void ToString_ContainsRoleAndName()
        {
            Assert.That(hacker.ToString(), Does.Contain("Hacker"));
            Assert.That(hacker.ToString(), Does.Contain("Ana Ruiz"));
        }
    }
}