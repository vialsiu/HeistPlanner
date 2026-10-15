using HeistPlanner.Models;
using Newtonsoft.Json;
using RestSharp;

namespace HeistPlanner.Services
{
    public class ArtworkService
    {
        //---- Ref: Cleveland Museum of Art Open Access API docs: https://openaccess-api.clevelandart.org/
        public static readonly string ARTWORK_SEARCH_URL = "https://openaccess-api.clevelandart.org/api/artworks/";
        public static readonly string ARTWORK_FIELDS =
            "id,title,creation_date,technique,department,culture,measurements,creators,images";
        public static List<Artwork> SearchArtworks(string searchText)
        {
            var client = new RestClient(ARTWORK_SEARCH_URL);
            var request = new RestRequest();

            request.AddParameter("q", searchText);
            request.AddParameter("has_image", 1);
            request.AddParameter("fields", ARTWORK_FIELDS);
            request.AddParameter("limit", 12);
            var response = client.Execute(request);

            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                throw new ApiException($"Couldn't reach the museum collection (status {(int)response.StatusCode}). Try again later.");            
            }

            return ParseArtworks(response.Content);
        }

        public static List<Artwork> ParseArtworks(string receivedJSON)
        {
            try
            {
                ArtworkSearchJSON? searchResponse = JsonConvert.DeserializeObject<ArtworkSearchJSON>(receivedJSON);
                return searchResponse?.data ?? new List<Artwork>();
            }
            catch (JsonException)
            {
                throw new ApiException("The artwork data came back in an unexpected format.");
            }
        }
    }

    public class ArtworkSearchJSON
    {
        public List<Artwork> data { get; set; } = new List<Artwork>();
    }
}