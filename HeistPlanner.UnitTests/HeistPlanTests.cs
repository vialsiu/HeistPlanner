using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    public class HeistPlanTests
    {
        private HeistPlan plan;
        private Artwork waterLilies;
        private Artwork starryNight;
        private Artwork theScream;
        private Artwork monaLisa;

        //---fresh plan and artworks before every test so the tests dont affect each other
        [SetUp]
        public void Setup()
        {
            plan = new HeistPlan();
            waterLilies = new Artwork(1, "Water Lilies", "Claude Monet");
            starryNight = new Artwork(2, "The Starry Night", "Vincent van Gogh");
            theScream = new Artwork(3, "The Scream", "Edvard Munch");
            monaLisa = new Artwork(4, "Mona Lisa", "Leonardo da Vinci");
        }

        [Test]
        public void NewPlan_HasNoTargets()
        {
            Assert.That(plan.Targets, Is.Empty);
        }

        //--- one test for each path through the method
        [Test]
        public void AddTarget_NewArtwork_ReturnsNull()
        {
            //---null means it was added with no problems
            Assert.That(plan.AddTarget(waterLilies), Is.Null);
        }

        [Test]
        public void AddTarget_NewArtwork_IsAddedToList()
        {
            plan.AddTarget(waterLilies);
            Assert.That(plan.Targets, Has.Exactly(1).Items);
        }

        [Test]
        public void AddTarget_Duplicate_ReturnsMessage()
        {
            plan.AddTarget(waterLilies);
            Assert.That(plan.AddTarget(waterLilies), Does.Contain("already on your target list"));
        }

        [Test]
        public void AddTarget_Duplicate_IsNotAddedTwice()
        {
            plan.AddTarget(waterLilies);
            plan.AddTarget(waterLilies);
            Assert.That(plan.Targets, Has.Exactly(1).Items);
        }

        [Test]
        public void AddTarget_SameIdDifferentObject_CountsAsDuplicate()
        {
            //---a new search makes new objects for the same painting, Equals compares by id so it still gets blocked
            Artwork sameWaterLilies = new Artwork(1, "Water Lilies", "Claude Monet");
            plan.AddTarget(waterLilies);
            Assert.That(plan.AddTarget(sameWaterLilies), Is.Not.Null);
        }

        //---boundary testing, the 3rd target is allowed but the 4th isnt
        [Test]
        public void AddTarget_ThirdTarget_IsAllowed()
        {
            plan.AddTarget(waterLilies);
            plan.AddTarget(starryNight);
            Assert.That(plan.AddTarget(theScream), Is.Null);
        }

        [Test]
        public void AddTarget_FourthTarget_ReturnsMessage()
        {
            plan.AddTarget(waterLilies);
            plan.AddTarget(starryNight);
            plan.AddTarget(theScream);
            Assert.That(plan.AddTarget(monaLisa), Does.Contain("can only carry"));
        }

        [Test]
        public void AddTarget_FourthTarget_ListStaysAtMax()
        {
            plan.AddTarget(waterLilies);
            plan.AddTarget(starryNight);
            plan.AddTarget(theScream);
            plan.AddTarget(monaLisa);
            Assert.That(plan.Targets.Count, Is.EqualTo(HeistPlan.MAX_TARGETS));
        }

        //---RemoveTarget and IsTarget
        [Test]
        public void RemoveTarget_OnList_IsRemoved()
        {
            plan.AddTarget(waterLilies);
            plan.RemoveTarget(waterLilies);
            Assert.That(plan.Targets, Is.Empty);
        }

        [Test]
        public void RemoveTarget_NotOnList_DoesNothing()
        {
            plan.AddTarget(waterLilies);
            plan.RemoveTarget(starryNight);
            Assert.That(plan.Targets, Has.Exactly(1).Items);
        }

        [Test]
        public void IsTarget_OnList_ReturnsTrue()
        {
            plan.AddTarget(waterLilies);
            Assert.That(plan.IsTarget(waterLilies), Is.True);
        }

        [Test]
        public void IsTarget_NotOnList_ReturnsFalse()
        {
            Assert.That(plan.IsTarget(waterLilies), Is.False);
        }
    }
}