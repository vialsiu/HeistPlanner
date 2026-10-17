using HeistPlanner.Models;
using Newtonsoft.Json;
using RestSharp;

namespace HeistPlanner.Services
{
    //---searches the collection and turns the JSON into Artwork objects
    public class ArtworkService
    {
        //---- ref: Cleveland Museum of Art Open Access API docs https://openaccess-api.clevelandart.org/
        public static readonly string ARTWORK_SEARCH_URL = "https://openaccess-api.clevelandart.org/api/artworks/";

        public static readonly string ARTWORK_FIELDS =
            "id,title,creation_date,technique,department,culture,measurements,creators,images";

        public static readonly int RESULTS_PER_PAGE = 12;

        public static List<Artwork> SearchArtworks(string searchText)
        {
            var request = new RestRequest();
            request.AddParameter("q", searchText);
            return GetArtworks(request);
        }

        //---highlight=1 gives the works the museum features on its own collection home page
        public static List<Artwork> GetFeaturedArtworks()
        {
            var request = new RestRequest();
            request.AddParameter("highlight", 1);
            return GetArtworks(request);
        }

        //---shared by both searches so the settings and the error checks are only written once
        private static List<Artwork> GetArtworks(RestRequest request)
        {
            var client = new RestClient(ARTWORK_SEARCH_URL);

            request.AddParameter("has_image", 1);
            request.AddParameter("fields", ARTWORK_FIELDS);
            request.AddParameter("limit", RESULTS_PER_PAGE);
            var response = client.Execute(request);

            //---RestSharp IsSuccessful true only if the request completed and the status code was ok https://restsharp.dev/docs/usage/response
            //---HttpStatusCode enum, cast to int to show the number https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                throw new ApiException(
                    $"Couldn't reach the museum collection (status {(int)response.StatusCode}). Try again later.");
            }

            return ParseArtworks(response.Content);
        }

        //---kept separate from the API call so it can be unit tested with sample JSON 
        public static List<Artwork> ParseArtworks(string receivedJSON)
        {
            try
            {
                ArtworkSearchJSON? searchResponse = JsonConvert.DeserializeObject<ArtworkSearchJSON>(receivedJSON);
                if (searchResponse != null && searchResponse.data != null)
                {
                    return searchResponse.data;
                }

                return new List<Artwork>();
            }
            //--- JsonException thrown when the JSON can't be read https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonException.htm
            catch (JsonException)
            {
                throw new ApiException("The artwork data came back in an unexpected format.");
            }
        }
    }

    //--Matches the outer JSON from the API
    public class ArtworkSearchJSON
    {
        public List<Artwork> data { get; set; } = new List<Artwork>();
    }
}