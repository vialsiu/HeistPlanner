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
        public void ParseArtworks_ValidJson_ReadsArtistAndImage()
        {
            List<Artwork> artworks = ArtworkService.ParseArtworks(SAMPLE_JSON);
            Assert.That(artworks[0].Artist, Is.EqualTo("Claude Monet"));
            Assert.That(artworks[0].ImageUrl, Is.EqualTo("https://example.com/water-lilies.jpg"));
        }

        //----bad JSON: should become the ApiException
        [Test]
        public void ParseArtworks_BrokenJson_ThrowsApiException()
        {
            Assert.That(() => ArtworkService.ParseArtworks("this is not json"), Throws.TypeOf<ApiException>());
        }

    }
}