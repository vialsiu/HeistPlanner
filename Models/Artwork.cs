using Newtonsoft.Json;

namespace HeistPlanner.Models
{
    //-- for Cleveland Museum of Art JSON
    public class ArtworkCreator
    {
        public string? description { get; set; }   //e.g. Claude Monet (French, 1840–1926)
    }

    public class ArtworkImages
    {
        public ArtworkImage? web { get; set; }
    }

    public class ArtworkImage
    {
        public string? url { get; set; }
    }

    public class Artwork : IComparable<Artwork>
    {
        private int id;
        private string title = "";
        private string? dateDisplay;
        private string? mediumDisplay;
        private string? departmentTitle;
        private string? dimensions;
        private List<string> culture = new List<string>();
        private List<ArtworkCreator> creators = new List<ArtworkCreator>();
        private ArtworkImages? images;

        //--Ref https://www.newtonsoft.com/json/help/html/JsonPropertyName.htm
        [JsonProperty("id")]
        public int Id { get { return id; } set { id = value; } }

        [JsonProperty("title")]
        public string Title { get { return title; } set { title = value; } }

        [JsonProperty("creation_date")]
        public string? DateDisplay { get { return dateDisplay; } set { dateDisplay = value; } }

        [JsonProperty("technique")]
        public string? MediumDisplay { get { return mediumDisplay; } set { mediumDisplay = value; } }

        [JsonProperty("department")]
        public string? DepartmentTitle { get { return departmentTitle; } set { departmentTitle = value; } }

        [JsonProperty("measurements")]
        public string? Dimensions { get { return dimensions; } set { dimensions = value; } }

        [JsonProperty("culture")]
        public List<string> Culture
        {
            get { return culture; }
            set { culture = value ?? new List<string>(); }
        }

        [JsonProperty("creators")]
        public List<ArtworkCreator> Creators
        {
            get { return creators; }
            set { creators = value ?? new List<ArtworkCreator>(); }
        }

        [JsonProperty("images")]
        public ArtworkImages? Images { get { return images; } set { images = value; } }

        public string Artist
        {
            get
            {
                if (creators.Count == 0 || string.IsNullOrWhiteSpace(creators[0].description))
                {
                    return "Unknown artist";
                }

                //--the API gives Claude Monet (French, 1840–1926), keep just the name before the bracket
                string description = creators[0].description!;
                int bracket = description.IndexOf("(");
                return bracket > 0 ? description.Substring(0, bracket).Trim() : description;
            }
        }

        public string PlaceOfOrigin
        {
            get { return culture.Count > 0 ? culture[0] : "Unknown"; }
        }

        public string ImageUrl
        {
            get { return images?.web?.url ?? ""; }
        }

        public Artwork()
        {
            //--so newtonsoft can create the object when deserializing
            id = 0;
            title = "Untitled";
        }

        public Artwork(int id, string title, string artist)
        {
            this.id = id;
            this.title = title;
            ArtworkCreator creator = new ArtworkCreator();
            creator.description = artist;
            creators.Add(creator);
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

        //--Ref: Microsoft guidelines for overloading == safely, https://learn.microsoft.com/en-us/previous-versions/ms173147(v=vs.90)
        //need this so == compares artworks by id instead of by memory reference, and so it doesn't crash
        //--when the left side is null - e.g. checking if an artwork is already on the target list
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

            //--sort alphabetically by title, then by artist if two titles match
            int result = string.Compare(title, other.title, ignoreCase: true);
            if (result == 0)
            {
                result = string.Compare(Artist, other.Artist, ignoreCase: true);
            }
            return result;
        }
    }
}