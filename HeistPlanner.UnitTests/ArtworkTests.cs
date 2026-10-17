using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    //---nunit assert reference: https://docs.nunit.org/articles/nunit/writing-tests/assertions/assertion-models/constraint.html
    public class ArtworkTests
    {
        private Artwork waterLilies;
        private Artwork sameIdDifferentTitle;
        private Artwork starryNight;

        //make each test start with fresh objects
        [SetUp]
        public void Setup()
        {
            waterLilies = new Artwork(1, "Water Lilies", "Claude Monet (French, 1840–1926)");
            sameIdDifferentTitle = new Artwork(1, "Something Else", "Someone Else");
            starryNight = new Artwork(2, "The Starry Night", "Vincent van Gogh (Dutch, 1853–1890)");
        }

        [Test]
        public void EmptyConstructor_CreatesArtwork()
        {
            Artwork artwork = new Artwork();
            Assert.That(artwork, Is.Not.Null);
        }

        [Test]
        public void Artist_RemovesNationalityAndDates()
        {
            Assert.That(waterLilies.Artist, Is.EqualTo("Claude Monet"));
        }

        [Test]
        public void Artist_NoBrackets_ReturnsWholeName()
        {
            Artwork artwork = new Artwork(3, "Untitled", "Banksy");
            Assert.That(artwork.Artist, Is.EqualTo("Banksy"));
        }

        [Test]
        public void Artist_NoCreators_ReturnsUnknownArtist()
        {
            Artwork artwork = new Artwork();
            Assert.That(artwork.Artist, Is.EqualTo("Unknown artist"));
        }

        //artworks are the same if they have the same museum id
        [Test]
        public void Equals_SameId_ReturnsTrue()
        {
            Assert.That(waterLilies.Equals(sameIdDifferentTitle), Is.True);
        }

        [Test]
        public void Equals_DifferentId_ReturnsFalse()
        {
            Assert.That(waterLilies.Equals(starryNight), Is.False);
        }

        [Test]
        public void Equals_NotAnArtwork_ReturnsFalse()
        {
            Assert.That(waterLilies.Equals("Water Lilies"), Is.False);
        }

        [Test]
        public void EqualsOperator_NullOnLeft_ReturnsFalseWithoutCrashing()
        {
            Artwork? nothing = null;
            Assert.That(nothing == waterLilies, Is.False);
        }

        [Test]
        public void GetHashCode_SameId_SameHashCode()
        {
            Assert.That(waterLilies.GetHashCode(), Is.EqualTo(sameIdDifferentTitle.GetHashCode()));
        }

        //-----------sort by title then by artist
        [Test]
        public void Sort_OrdersByTitle()
        {
            List<Artwork> artworks = new List<Artwork>();
            artworks.Add(starryNight);
            artworks.Add(waterLilies);

            artworks.Sort();

            Assert.That(artworks[0].Title, Is.EqualTo("The Starry Night"));
            Assert.That(artworks[1].Title, Is.EqualTo("Water Lilies"));
        }

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