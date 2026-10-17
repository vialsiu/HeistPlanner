namespace HeistPlanner.Models
{
    //---the target building, the sun and weather services both use this so the location is only written once
    public class Museum
    {
        public static readonly string NAME = "Cleveland Museum of Art";

        //---strings not doubles so the decimal point is always a full stop cause some languages write numbers with a comma
        public static readonly string LATITUDE = "41.5089";
        public static readonly string LONGITUDE = "-81.6119";
        public static readonly string TIMEZONE = "America/New_York";
    }
}