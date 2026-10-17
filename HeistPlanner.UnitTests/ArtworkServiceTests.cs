using HeistPlanner.Models;
using HeistPlanner.Services;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    //-----these tests never call the real API 
    public class ArtworkServiceTests
    {
        //---- i'm using raw string literals (""") so the JSON doesn't need escaped quotes - https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/raw-string
        private static readonly string SAMPLE_JSON = """
            {
              "info": { "total": 2 },
              "data": [
                {
                  "id": 1001,
                  "title": "Water Lilies",
                  "creation_date": "c. 1915-1926",
                  "technique": "oil on canvas",
                  "department": "Modern European Painting and Sculpture",
                  "culture": [ "France, 20th century" ],
                  "measurements": "200 x 425 cm",
                  "creators": [ { "description": "Claude Monet (French, 1840-1926)" } ],
                  "images": { "web": { "url": "https://example.com/water-lilies.jpg" } }
                },
                {
                  "id": 1002,
                  "title": "Unknown Piece",
                  "creators": [],
                  "culture": null
                }
              ]
            }
            """;

        [Test]
        public void ParseArtworks_ValidJson_ReturnsEveryArtwork()
        {
            List<Artwork> artworks = ArtworkService.ParseArtworks(SAMPLE_JSON);
            Assert.That(artworks, Has.Exactly(2).Items);
        }

        [Test]
        public void ParseArtworks_ValidJson_ReadsIdAndTitle()
        {
            List<Artwork> artworks = ArtworkService.ParseArtworks(SAMPLE_JSON);
            Assert.That(artworks[0].Id, Is.EqualTo(1001));
            Assert.That(artworks[0].Title, Is.EqualTo("Water Lilies"));
        }

        [Test]
        public void ParseArtworks_ValidJson_ReadsArtistAndImage()
        {
            List<Artwork> artworks = ArtworkService.ParseArtworks(SAMPLE_JSON);
            Assert.That(artworks[0].Artist, Is.EqualTo("Claude Monet"));
            Assert.That(artworks[0].ImageUrl, Is.EqualTo("https://example.com/water-lilies.jpg"));
        }

        //----so the Artwork class falls back to defaults instead of crash
        [Test]
        public void ParseArtworks_MissingFields_UsesDefaults()
        {
            List<Artwork> artworks = ArtworkService.ParseArtworks(SAMPLE_JSON);
            Assert.That(artworks[1].Artist, Is.EqualTo("Unknown artist"));
            Assert.That(artworks[1].PlaceOfOrigin, Is.EqualTo("Unknown"));
            Assert.That(artworks[1].ImageUrl, Is.EqualTo(""));
        }

        //----empty results
        [Test]
        public void ParseArtworks_EmptyData_ReturnsEmptyList()
        {
            List<Artwork> artworks = ArtworkService.ParseArtworks("""{ "data": [] }""");
            Assert.That(artworks, Is.Empty);
        }

        [Test]
        public void ParseArtworks_NullJson_ReturnsEmptyList()
        {
            List<Artwork> artworks = ArtworkService.ParseArtworks("null");
            Assert.That(artworks, Is.Empty);
        }

        //----bad JSON: should become the ApiException
        [Test]
        public void ParseArtworks_BrokenJson_ThrowsApiException()
        {
            Assert.That(() => ArtworkService.ParseArtworks("this is not json"), Throws.TypeOf<ApiException>());
        }

        [Test]
        public void ParseArtworks_WrongShape_ThrowsApiException()
        {
            Assert.That(() => ArtworkService.ParseArtworks("""{ "data": "not a list" }"""), Throws.TypeOf<ApiException>());
        }
    }
}