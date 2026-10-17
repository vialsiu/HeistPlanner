using System.Globalization;
using HeistPlanner.Models;
using Newtonsoft.Json;
using RestSharp;

namespace HeistPlanner.Services
{
    //---gets the hourly weather forecast at the museum from Open-Meteo and picks the hour of the heist
    //----ref Open-Meteo forecast API docs https://open-meteo.com/en/docs
    public class WeatherService
    {
        public static readonly string WEATHER_URL = "https://api.open-meteo.com/v1/forecast";
        public static readonly string HOURLY_FIELDS = "temperature_2m,precipitation_probability,cloud_cover,visibility,wind_speed_10m,weather_code";

        //---the forecast goes 16 days ahead and the go time can be after midnight (the next day), so 14 to be safe
        public static readonly int MAX_DAYS_AHEAD = 14;

        public static WeatherReport GetWeatherAt(DateTime goTime)
        {
            var client = new RestClient(WEATHER_URL);
            var request = new RestRequest();

            string day = goTime.ToString("yyyy-MM-dd");
            request.AddParameter("latitude", Museum.LATITUDE);
            request.AddParameter("longitude", Museum.LONGITUDE);
            request.AddParameter("hourly", HOURLY_FIELDS);
            request.AddParameter("timezone", Museum.TIMEZONE);
            request.AddParameter("start_date", day);
            request.AddParameter("end_date", day);
            var response = client.Execute(request);

            //----ref RestSharp IsSuccessful https://restsharp.dev/docs/usage/response
            //----ref HttpStatusCode cast to int to show the number https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                throw new ApiException($"Couldn't get the weather forecast (status {(int)response.StatusCode}). Try again later.");
            }

            WeatherForecastJSON forecast = ParseForecast(response.Content);
            return PickHour(forecast, goTime);
        }

        //---null means the date is ok, otherwise a message saying why not
        //---today is passed in so the tests can pretend any day is today
        public static string? CheckDate(DateTime nightOf, DateTime today)
        {
            if (nightOf.Date < today.Date)
            {
                return "That night has already passed. Pick tonight or a night coming up.";
            }

            if (nightOf.Date > today.Date.AddDays(MAX_DAYS_AHEAD))
            {
                return $"The weather forecast only goes {MAX_DAYS_AHEAD} days ahead. Pick a night before {today.Date.AddDays(MAX_DAYS_AHEAD + 1):d MMM}.";
            }

            return null;
        }

        //---separate from the API call so it can be unit tested with sample json
        public static WeatherForecastJSON ParseForecast(string receivedJSON)
        {
            try
            {
                //----ref DateParseHandling.None keeps the times as text so Newtonsoft doesnt change them - https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonSerializerSettings_DateParseHandling.htm
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DateParseHandling = DateParseHandling.None;

                WeatherForecastJSON? forecast = JsonConvert.DeserializeObject<WeatherForecastJSON>(receivedJSON, settings);
                if (forecast == null || forecast.hourly == null || forecast.hourly.time.Count == 0)
                {
                    throw new ApiException("The weather forecast came back empty.");
                }
                return forecast;
            }
            //----ref JsonException https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonException.htm
            catch (JsonException)
            {
                throw new ApiException("The weather forecast came back in an unexpected format.");
            }
        }

        //---the API gives a list of times and a list for each value in the same order,
        //---so find the position of the go time and read every list at that position
        public static WeatherReport PickHour(WeatherForecastJSON forecast, DateTime goTime)
        {
            WeatherHourlyJSON hourly = forecast.hourly;
            DateTime wantedHour = goTime.Date.AddHours(goTime.Hour);

            for (int i = 0; i < hourly.time.Count; i++)
            {
                //----ref CultureInfo.InvariantCulture so the time is read the same way on any computer https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture
                DateTime hour = DateTime.Parse(hourly.time[i], CultureInfo.InvariantCulture);
                if (hour == wantedHour)
                {
                    return new WeatherReport(
                        hour,
                        ReadValue(hourly.temperature_2m, i),
                        (int)ReadValue(hourly.precipitation_probability, i),
                        (int)ReadValue(hourly.cloud_cover, i),
                        ReadValue(hourly.visibility, i),
                        ReadValue(hourly.wind_speed_10m, i),
                        (int)ReadValue(hourly.weather_code, i));
                }
            }

            throw new ApiException($"The forecast doesn't have the weather for {goTime:ddd d MMM h:mmtt}.");
        }

        //---some hours come back as null in the forecast, treat those as 0
        //----ref nullable value types (double?), HasValue and Value https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types
        private static double ReadValue(List<double?> values, int index)
        {
            if (index < values.Count && values[index].HasValue)
            {
                return values[index].Value;
            }
            return 0;
        }
    }

    //---classes matching the Open-Meteo json, lowercase names match the json
    public class WeatherForecastJSON
    {
        public string timezone { get; set; } = "";
        public WeatherHourlyJSON hourly { get; set; } = new WeatherHourlyJSON();
    }

    //---every list is in the same order as time, so position 5 in each list is the same hour
    public class WeatherHourlyJSON
    {
        public List<string> time { get; set; } = new List<string>();
        public List<double?> temperature_2m { get; set; } = new List<double?>();
        public List<double?> precipitation_probability { get; set; } = new List<double?>();
        public List<double?> cloud_cover { get; set; } = new List<double?>();
        public List<double?> visibility { get; set; } = new List<double?>();
        public List<double?> wind_speed_10m { get; set; } = new List<double?>();
        public List<double?> weather_code { get; set; } = new List<double?>();
    }
}