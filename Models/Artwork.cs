using Newtonsoft.Json;

namespace HeistPlanner.Models
{
    public class Artwork : IComparable<Artwork>
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

        //-----Ref: newtonsoft JsonProperty maps the API's snake_case names https://www.newtonsoft.com/json/help/html/JsonPropertyName.htm
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

        //--------Ref: AIC IIIF image URL format: https://api.artic.edu/docs/#images
        public string ImageUrl
        {
            get { return imageId == null ? "" : $"https://www.artic.edu/iiif/2/{imageId}/full/843,/0/default.jpg"; }
        }

        public Artwork()
        {
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

        public override bool Equals(object? obj)
        {
            if (!(obj is Artwork))
            {
                return false;
            }

            var other = (Artwork)obj;
            return other.id == id;
        }
        //-------Ref: Microsoft guidelines for overloading == safely, https://learn.microsoft.com/en-us/previous-versions/ms173147(v=vs.90)
        //-------I need this so == compares artworks by ID instead of by memory ref, and so it doesn't crash
        //-------when the left side is null e.g. checking if an artwork is already on the target list
        public static bool operator ==(Artwork? left, Artwork? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Artwork? left, Artwork? right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public int CompareTo(Artwork? other)
        {
            if (other is null) return 1;

            int result = string.Compare(title, other.title, ignoreCase: true);
            if (result == 0)
            {
                result = string.Compare(Artist, other.Artist, ignoreCase: true);
            }
            return result;
        }
    }
}