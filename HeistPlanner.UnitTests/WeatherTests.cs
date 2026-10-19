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

        //---GetCover, fog and the 70% cloud boundary

        [Test]
        public void GetCover_Fog_IsExcellent()
        {
            Assert.That(MakeWeather(45, 100, 500, 0).GetCover(), Is.EqualTo(WeatherCover.Excellent));
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

        //---CheckDate, boundary testing on both ends

        [Test]
        public void CheckDate_FifteenDaysAhead_ReturnsMessage()
        {
            Assert.That(WeatherService.CheckDate(today.AddDays(15), today), Does.Contain("only goes"));
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

    }
}