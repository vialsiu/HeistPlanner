using Newtonsoft.Json;

namespace HeistPlanner.Models
{
    public class Artwork
    {
        private int id;
        private string title = "";
        private string? artistTitle;
        private string? dateDisplay;
        private string? mediumDisplay;
        private string? placeOfOrigin;
        private string? dimensions;
        private string? departmentTitle;
        private string? imageId;
        
        [JsonProperty("id")]
        public int Id { get { return id; } set { id = value; } }

        [JsonProperty("title")]
        public string Title { get { return title; } set { title = value; } }

        [JsonProperty("artist_title")]
        public string? ArtistTitle { get { return artistTitle; } set { artistTitle = value; } }

        [JsonProperty("date_display")]
        public string? DateDisplay { get { return dateDisplay; } set { dateDisplay = value; } }

        [JsonProperty("medium_display")]
        public string? MediumDisplay { get { return mediumDisplay; } set { mediumDisplay = value; } }

        [JsonProperty("place_of_origin")]
        public string? PlaceOfOrigin { get { return placeOfOrigin; } set { placeOfOrigin = value; } }

        [JsonProperty("dimensions")]
        public string? Dimensions { get { return dimensions; } set { dimensions = value; } }

        [JsonProperty("department_title")]
        public string? DepartmentTitle { get { return departmentTitle; } set { departmentTitle = value; } }

        [JsonProperty("image_id")]
        public string? ImageId { get { return imageId; } set { imageId = value; } }

        public string Artist
        {
            get { return string.IsNullOrWhiteSpace(artistTitle) ? "Unknown artist" : artistTitle; }
        }

        public string ImageUrl
        {
            get { return imageId == null ? "" : $"https://www.artic.edu/iiif/2/{imageId}/full/843,/0/default.jpg"; }
        }

        public Artwork()
        {
            //Needed so Newtonsoft can create the object when deserializing
            id = 0;
            title = "Untitled";
        }

        public Artwork(int id, string title, string artistTitle)
        {
            this.id = id;
            this.title = title;
            this.artistTitle = artistTitle;
        }

        public override string ToString()
        {
            return $"{title} by {Artist} ({dateDisplay})";
        }
    }
}