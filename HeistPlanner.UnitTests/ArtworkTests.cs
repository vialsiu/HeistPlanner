using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    public class ArtworkTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EmptyConstructor_CreatesArtwork()
        {
            Artwork artwork = new Artwork();
            Assert.That(artwork, Is.Not.Null);
        }
    }
}