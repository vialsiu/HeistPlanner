namespace HeistPlanner.Models
{
    public class NightReport
    {
        public static readonly double BRIGHT_MOON_PERCENT = 50;   //---over half the moon lit up = too much light

        private DateTime nightOf;
        private DateTime sunset;
        private DateTime darkStarts;        //-----astronomical twilight ends, the sky is fully dark
        private DateTime darkEnds;          //----astronomical twilight begins the next morning
        private DateTime sunrise;           //---the next morning
        private string moonPhase;
        private double moonIllumination;    //--------0 to 100 %

        //Properties (public, PascalCase)
        public DateTime NightOf
        {
            get { return nightOf; }
            set { nightOf = value; }
        }

        public DateTime Sunset
        {
            get { return sunset; }
            set { sunset = value; }
        }

        public DateTime DarkStarts
        {
            get { return darkStarts; }
            set { darkStarts = value; }
        }

        public DateTime DarkEnds
        {
            get { return darkEnds; }
            set { darkEnds = value; }
        }

        public DateTime Sunrise
        {
            get { return sunrise; }
            set { sunrise = value; }
        }

        public string MoonPhase
        {
            get { return moonPhase; }
            set { moonPhase = value; }
        }

        public double MoonIllumination
        {
            get { return moonIllumination; }
            set { moonIllumination = value; }
        }

        //Constructors
        public NightReport()
        {
            nightOf = DateTime.Today;
            sunset = DateTime.Today;
            darkStarts = DateTime.Today;
            darkEnds = DateTime.Today;
            sunrise = DateTime.Today;
            moonPhase = "unknown";
            moonIllumination = 0;
        }

        public NightReport(DateTime nightOf, DateTime sunset, DateTime darkStarts, DateTime darkEnds,
            DateTime sunrise, string moonPhase, double moonIllumination)
        {
            this.nightOf = nightOf;
            this.sunset = sunset;
            this.darkStarts = darkStarts;
            this.darkEnds = darkEnds;
            this.sunrise = sunrise;
            this.moonPhase = moonPhase;
            this.moonIllumination = moonIllumination;
        }

        public bool IsDarkAt(DateTime time)
        {
            return time >= darkStarts && time <= darkEnds;
        }

        //----how many hours of full darkness there are, e.g. 9.3
        public double GetDarkHours()
        {
            //----ref subtracting two DateTimes gives a TimeSpan, TotalHours turns it into a number of hours https://learn.microsoft.com/en-us/dotnet/api/system.timespan
            TimeSpan length = darkEnds - darkStarts;
            return Math.Round(length.TotalHours, 1);
        }

        public bool IsBrightMoon()
        {
            return moonIllumination >= BRIGHT_MOON_PERCENT;
        }

        //---the user picks a go time as an hour, anything before midday means after midnight so its the next day
        public static DateTime GetGoTime(DateTime nightOf, int hour)
        {
            if (hour < 12)
            {
                return nightOf.Date.AddDays(1).AddHours(hour);
            }
            return nightOf.Date.AddHours(hour);
        }

        public override string ToString()
        {
            return $"Night of {nightOf:ddd d MMM}: dark from {darkStarts:h:mmtt} to {darkEnds:h:mmtt}, moon {moonIllumination}% ({moonPhase})";
        }
    }
}