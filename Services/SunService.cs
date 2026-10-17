using System.Globalization;
using HeistPlanner.Models;
using Newtonsoft.Json;
using RestSharp;

namespace HeistPlanner.Services
{
    //---gets sunset, darkness and moon info for the museum from the sunrise sunset API
    //-----ref sunrise sunset API docs, free with no key, needs a credit to sunrise-sunset.org https://sunrise-sunset.org/api
    public class SunService
    {
        public static readonly string SUN_URL = "https://api.sunrise-sunset.org/v2";

        //---a heist night goes past midnight so it needs the evening of one day and the morning of the next
        public static NightReport GetNight(DateTime nightOf)
        {
            SunDayJSON evening = GetSunDay(nightOf);
            SunDayJSON morning = GetSunDay(nightOf.AddDays(1));
            return CreateNightReport(nightOf, evening, morning);
        }

        private static SunDayJSON GetSunDay(DateTime date)
        {
            var client = new RestClient(SUN_URL);
            var request = new RestRequest();

            request.AddParameter("lat", Museum.LATITUDE);
            request.AddParameter("lng", Museum.LONGITUDE);
            request.AddParameter("date", date.ToString("yyyy-MM-dd"));
            request.AddParameter("tz", Museum.TIMEZONE);
            var response = client.Execute(request);

            //----ref RestSharp IsSuccessful https://restsharp.dev/docs/usage/response
            //----ref HttpStatusCode cast to int to show the number https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                throw new ApiException($"Couldn't get the sunset times (status {(int)response.StatusCode}). Try again later.");
            }

            return ParseSunDay(response.Content);
        }

        //---separate from the API call so it can be unit tested with sample json
        public static SunDayJSON ParseSunDay(string receivedJSON)
        {
            try
            {
                //----ref stops Newtonsoft turning the time text into a DateTime in my computers timezone, I want the museums clock time https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonSerializerSettings_DateParseHandling.htm
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DateParseHandling = DateParseHandling.None;

                SunDayJSON? day = JsonConvert.DeserializeObject<SunDayJSON>(receivedJSON, settings);
                if (day == null || string.IsNullOrWhiteSpace(day.sunset))
                {
                    throw new ApiException("The sunset data came back empty.");
                }
                return day;
            }
            //----ref Newtonsoft JsonException https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonException.htm
            catch (JsonException)
            {
                throw new ApiException("The sunset data came back in an unexpected format.");
            }
        }

        //---puts the evening and the next morning together into one night
        public static NightReport CreateNightReport(DateTime nightOf, SunDayJSON evening, SunDayJSON morning)
        {
            return new NightReport(
                nightOf.Date,
                ReadTime(evening.sunset),
                ReadTime(evening.astronomical_twilight_end),
                ReadTime(morning.astronomical_twilight_begin),
                ReadTime(morning.sunrise),
                evening.moon_phase,
                evening.moon_illumination);
        }

        //---"2026-10-24T19:58:10-04:00" becomes 24/10/2026 19:58:10, the clock time at the museum
        public static DateTime ReadTime(string? isoTime)
        {
            //---the API leaves a time out if it never happens that day, for example if it never gets fully dark
            if (string.IsNullOrWhiteSpace(isoTime))
            {
                throw new ApiException("It doesn't get fully dark at the museum that night. Pick another date.");
            }

            try
            {
                //----ref DateTimeOffset.Parse keeps the -04:00 from the text, .DateTime gives the clock time without moving it to my timezone https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.parse
                //----ref CultureInfo.InvariantCulture so the date is read the same way on any computer https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture
                return DateTimeOffset.Parse(isoTime, CultureInfo.InvariantCulture).DateTime;
            }
            //----ref FormatException is what Parse throws when the text isnt a date https://learn.microsoft.com/en-us/dotnet/api/system.formatexception
            catch (FormatException)
            {
                throw new ApiException($"Couldn't read the time \"{isoTime}\" from the sunset data.");
            }
        }
    }

    //---one day from the sunrise sunset API, lowercase names match the json
    //---times stay as text here, ReadTime turns them into DateTimes
    public class SunDayJSON
    {
        public string date { get; set; } = "";
        public string? sunrise { get; set; }
        public string? sunset { get; set; }
        public string? astronomical_twilight_begin { get; set; }
        public string? astronomical_twilight_end { get; set; }
        public string sun_status { get; set; } = "normal";
        public string moon_phase { get; set; } = "unknown";
        public double moon_illumination { get; set; }
    }
}