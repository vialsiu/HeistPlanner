namespace HeistPlanner.Models
{
    //--------------------how much the weather hides the crew
    public enum WeatherCover
    {
        Excellent,
        Good,
        Poor
    }

    //-------the weather at the museum for one hour (built from the Open-Meteo API)
    public class WeatherReport
    {
        public static readonly int HEAVY_CLOUD_PERCENT = 70;
        public static readonly double LOW_VISIBILITY_METRES = 5000;
        public static readonly int LIKELY_RAIN_PERCENT = 60;   //---60% chance or more counts as rain cover
        private DateTime time;
        private double temperature;         //---°C
        private int rainChance;             //---%
        private int cloudCover;             //---%
        private double visibility;          //---metres
        private double windSpeed;           //---km/h
        private int weatherCode;            //---WMO code

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        public double Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }

        public int RainChance
        {
            get { return rainChance; }
            set { rainChance = value; }
        }

        public int CloudCover
        {
            get { return cloudCover; }
            set { cloudCover = value; }
        }

        public double Visibility
        {
            get { return visibility; }
            set { visibility = value; }
        }

        public double WindSpeed
        {
            get { return windSpeed; }
            set { windSpeed = value; }
        }

        public int WeatherCode
        {
            get { return weatherCode; }
            set { weatherCode = value; }
        }

        public WeatherReport()
        {
            time = DateTime.Today;
            temperature = 0;
            rainChance = 0;
            cloudCover = 0;
            visibility = 0;
            windSpeed = 0;
            weatherCode = 0;
        }

        public WeatherReport(DateTime time, double temperature, int rainChance, int cloudCover,
            double visibility, double windSpeed, int weatherCode)
        {
            this.time = time;
            this.temperature = temperature;
            this.rainChance = rainChance;
            this.cloudCover = cloudCover;
            this.visibility = visibility;
            this.windSpeed = windSpeed;
            this.weatherCode = weatherCode;
        }

        //----ref WMO weather codes table from the Open-Meteo docs https://open-meteo.com/en/docs
        public string GetDescription()
        {
            switch (weatherCode)
            {
                case 0:
                    return "Clear sky";
                case 1:
                    return "Mainly clear";
                case 2:
                    return "Partly cloudy";
                case 3:
                    return "Overcast";
                case 45:
                case 48:
                    return "Fog";
                case 51:
                case 53:
                case 55:
                case 56:
                case 57:
                    return "Drizzle";
                case 61:
                case 66:
                case 80:
                    return "Light rain";
                case 63:
                case 81:
                    return "Moderate rain";
                case 65:
                case 67:
                case 82:
                    return "Heavy rain";
                case 71:
                case 73:
                case 75:
                case 77:
                case 85:
                case 86:
                    return "Snow";
                case 95:
                case 96:
                case 97:
                case 99:
                    return "Thunderstorm";
                default:
                    return "Unknown";
            }
        }

        //---fog, rain or snow hide the crew best, a clear night is the worst
        public WeatherCover GetCover()
        {
            string description = GetDescription();
            if (description == "Fog" || description == "Drizzle" || description.Contains("rain") || description == "Snow"
                || rainChance >= LIKELY_RAIN_PERCENT)
            {
                return WeatherCover.Excellent;
            }

            if (description == "Thunderstorm" || cloudCover >= HEAVY_CLOUD_PERCENT || visibility < LOW_VISIBILITY_METRES)
            {
                return WeatherCover.Good;
            }

            return WeatherCover.Poor;
        }

        //---same idea as GetDegreeLevelName in Teacher.cs
        public static string GetCoverName(WeatherCover cover)
        {
            switch (cover)
            {
                case WeatherCover.Excellent:
                    return "Excellent cover";
                case WeatherCover.Good:
                    return "Some cover";
                case WeatherCover.Poor:
                    return "No cover";
                default:
                    return "Unknown";
            }
        }

        //Override methods
        public override string ToString()
        {
            return $"{time:h:mmtt}: {GetDescription()}, {temperature}°C, cloud {cloudCover}%, {GetCoverName(GetCover())}";
        }
    }
}