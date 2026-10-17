using HeistPlanner.Models;
using HeistPlanner.Services;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    //---tests for WeatherReport (cover rules) and WeatherService (date check, parsing, picking the hour)
    //---no real API calls, sample json in the same shape as Open-Meteo's hourly forecast
    public class WeatherTests
    {
        private static readonly string FORECAST_JSON = """
            {
              "latitude": 41.5,
              "longitude": -81.6,
              "timezone": "America/New_York",
              "hourly": {
                "time": ["2026-10-31T00:00", "2026-10-31T01:00", "2026-10-31T02:00", "2026-10-31T03:00"],
                "temperature_2m": [12.1, 11.8, 11.5, 11.0],
                "precipitation_probability": [10, 20, null, 40],
                "cloud_cover": [100, 95, 90, 85],
                "visibility": [24140, 20000, 3000, 15000],
                "wind_speed_10m": [18.0, 17.5, 16.9, 15.0],
                "weather_code": [3, 3, 45, 61]
              }
            }
            """;

        private DateTime today;

        [SetUp]
        public void Setup()
        {
            //---a fixed "today" so the date tests give the same result every time they run
            today = new DateTime(2026, 10, 17);
        }

        //---helper so each test only has to say what matters to it
        private WeatherReport MakeWeather(int code, int cloud, double visibility, int rainChance)
        {
            return new WeatherReport(new DateTime(2026, 10, 31, 1, 0, 0), 10, rainChance, cloud, visibility, 15, code);
        }

        //---GetDescription
        [Test]
        public void GetDescription_CodeZero_IsClearSky()
        {
            Assert.That(MakeWeather(0, 0, 24000, 0).GetDescription(), Is.EqualTo("Clear sky"));
        }

        [Test]
        public void GetDescription_Code45_IsFog()
        {
            Assert.That(MakeWeather(45, 100, 500, 0).GetDescription(), Is.EqualTo("Fog"));
        }

        [Test]
        public void GetDescription_Code63_IsModerateRain()
        {
            Assert.That(MakeWeather(63, 100, 8000, 80).GetDescription(), Is.EqualTo("Moderate rain"));
        }

        [Test]
        public void GetDescription_UnknownCode_IsUnknown()
        {
            Assert.That(MakeWeather(999, 0, 24000, 0).GetDescription(), Is.EqualTo("Unknown"));
        }

        [Test]
        public void GetCover_Fog_IsExcellent()
        {
            Assert.That(MakeWeather(45, 100, 500, 0).GetCover(), Is.EqualTo(WeatherCover.Excellent));
        }

        [Test]
        public void GetCover_Rain_IsExcellent()
        {
            Assert.That(MakeWeather(61, 100, 9000, 70).GetCover(), Is.EqualTo(WeatherCover.Excellent));
        }

        [Test]
        public void GetCover_Snow_IsExcellent()
        {
            Assert.That(MakeWeather(73, 100, 4000, 50).GetCover(), Is.EqualTo(WeatherCover.Excellent));
        }

        [Test]
        public void GetCover_HighRainChance_IsExcellent()
        {
            //---overcast and dry this hour but 60% chance of rain
            Assert.That(MakeWeather(3, 100, 24000, 60).GetCover(), Is.EqualTo(WeatherCover.Excellent));
        }

        [Test]
        public void GetCover_RainChanceFiftyNine_IsNotExcellent()
        {
            Assert.That(MakeWeather(0, 0, 24000, 59).GetCover(), Is.EqualTo(WeatherCover.Poor));
        }

        [Test]
        public void GetCover_Thunderstorm_IsGood()
        {
            Assert.That(MakeWeather(95, 100, 10000, 40).GetCover(), Is.EqualTo(WeatherCover.Good));
        }

        [Test]
        public void GetCover_SeventyPercentCloud_IsGood()
        {
            Assert.That(MakeWeather(3, 70, 24000, 0).GetCover(), Is.EqualTo(WeatherCover.Good));
        }

        [Test]
        public void GetCover_SixtyNinePercentCloud_IsPoor()
        {
            Assert.That(MakeWeather(2, 69, 24000, 0).GetCover(), Is.EqualTo(WeatherCover.Poor));
        }

        [Test]
        public void GetCover_LowVisibility_IsGood()
        {
            Assert.That(MakeWeather(0, 0, 4999, 0).GetCover(), Is.EqualTo(WeatherCover.Good));
        }

        [Test]
        public void GetCover_ClearNight_IsPoor()
        {
            Assert.That(MakeWeather(0, 0, 24000, 0).GetCover(), Is.EqualTo(WeatherCover.Poor));
        }

        [Test]
        public void GetCoverName_Poor_IsNoCover()
        {
            Assert.That(WeatherReport.GetCoverName(WeatherCover.Poor), Is.EqualTo("No cover"));
        }

        //---CheckDate, boundary testing on both ends
        [Test]
        public void CheckDate_Yesterday_ReturnsMessage()
        {
            Assert.That(WeatherService.CheckDate(today.AddDays(-1), today), Does.Contain("already passed"));
        }

        [Test]
        public void CheckDate_Today_IsAllowed()
        {
            Assert.That(WeatherService.CheckDate(today, today), Is.Null);
        }

        [Test]
        public void CheckDate_FourteenDaysAhead_IsAllowed()
        {
            Assert.That(WeatherService.CheckDate(today.AddDays(14), today), Is.Null);
        }

        [Test]
        public void CheckDate_FifteenDaysAhead_ReturnsMessage()
        {
            Assert.That(WeatherService.CheckDate(today.AddDays(15), today), Does.Contain("only goes"));
        }

        //---ParseForecast
        [Test]
        public void ParseForecast_ValidJson_ReadsEveryHour()
        {
            WeatherForecastJSON forecast = WeatherService.ParseForecast(FORECAST_JSON);
            Assert.That(forecast.hourly.time, Has.Exactly(4).Items);
        }

        [Test]
        public void ParseForecast_NoHours_ThrowsApiException()
        {
            Assert.That(() => WeatherService.ParseForecast("""{ "hourly": { "time": [] } }"""), Throws.TypeOf<ApiException>());
        }

        [Test]
        public void ParseForecast_BrokenJson_ThrowsApiException()
        {
            Assert.That(() => WeatherService.ParseForecast("{ hourly: oops"), Throws.TypeOf<ApiException>());
        }

        //---PickHour finds the go time in the lists
        [Test]
        public void PickHour_GoTime_ReadsThatHoursValues()
        {
            WeatherForecastJSON forecast = WeatherService.ParseForecast(FORECAST_JSON);
            WeatherReport weather = WeatherService.PickHour(forecast, new DateTime(2026, 10, 31, 2, 0, 0));

            Assert.That(weather.WeatherCode, Is.EqualTo(45));
            Assert.That(weather.Visibility, Is.EqualTo(3000));
            Assert.That(weather.Temperature, Is.EqualTo(11.5));
        }

        [Test]
        public void PickHour_NullValue_BecomesZero()
        {
            //---the rain chance at 2am is null in the sample json
            WeatherForecastJSON forecast = WeatherService.ParseForecast(FORECAST_JSON);
            WeatherReport weather = WeatherService.PickHour(forecast, new DateTime(2026, 10, 31, 2, 0, 0));
            Assert.That(weather.RainChance, Is.EqualTo(0));
        }

        [Test]
        public void PickHour_HourNotInForecast_ThrowsApiException()
        {
            WeatherForecastJSON forecast = WeatherService.ParseForecast(FORECAST_JSON);
            Assert.That(() => WeatherService.PickHour(forecast, new DateTime(2026, 10, 31, 22, 0, 0)), Throws.TypeOf<ApiException>());
        }
    }
}