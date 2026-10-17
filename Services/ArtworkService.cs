using HeistPlanner.Models;
using Newtonsoft.Json;
using RestSharp;

namespace HeistPlanner.Services
{
    //------- searches the cleveland art collection and turns the JSON into artwork objects
    public class ArtworkService
    {
        //---- reference is cleveland museum of art open access API docs: https://openaccess-api.clevelandart.org/
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

            //----RestSharp IsSuccessful true only if the request completed and the status code was ok: https://restsharp.dev/docs/usage/response
            //-----httpStatusCode enum, cast to int to show the number: https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode
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
                if (searchResponse != null && searchResponse.data != null)
                {
                    return searchResponse.data;
                }
                return new List<Artwork>();
            }
            //---thrown when the JSON can't be read: https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonException.htm
            catch (JsonException)
            {
                throw new ApiException("The artwork data came back in an unexpected format.");
            }
        }
    }

    //----matches the outer JSON from the API
    public class ArtworkSearchJSON
    {
        public List<Artwork> data { get; set; } = new List<Artwork>();
    }
}