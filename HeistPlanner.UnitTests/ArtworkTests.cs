using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    //---nunit assert reference: https://docs.nunit.org/articles/nunit/writing-tests/assertions/assertion-models/constraint.html
    public class ArtworkTests
    {
        private Artwork waterLilies;

        //make each test start with fresh objects
        [SetUp]
        public void Setup()
        {
            waterLilies = new Artwork(1, "Water Lilies", "Claude Monet (French, 1840–1926)");
        }

        [Test]
        public void Artist_RemovesNationalityAndDates()
        {
            Assert.That(waterLilies.Artist, Is.EqualTo("Claude Monet"));
        }

        //artworks are the same if they have the same museum id

        [Test]
        public void EqualsOperator_NullOnLeft_ReturnsFalseWithoutCrashing()
        {
            Artwork? nothing = null;
            Assert.That(nothing == waterLilies, Is.False);
        }

        //-----------sort by title then by artist

        [Test]
        public void CompareTo_SameTitle_OrdersByArtist()
        {
            Artwork degasVersion = new Artwork(10, "Dancers", "Edgar Degas (French, 1834–1917)");
            Artwork cezanneVersion = new Artwork(11, "Dancers", "Paul Cézanne (French, 1839–1906)");

            //---match first the title then artist like forexample "Edgar Degas" comes before "Paul Cézanne"
            Assert.That(degasVersion.CompareTo(cezanneVersion), Is.LessThan(0));
        }
    }
}