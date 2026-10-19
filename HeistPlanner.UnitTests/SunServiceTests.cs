using HeistPlanner.Models;
using HeistPlanner.Services;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    //---no real API calls, sample json in the same shape as the sunrise/sunset response for one day
    public class SunServiceTests
    {
        private static readonly string EVENING_JSON = """
            {
              "date": "2026-10-24",
              "tzid": "America/New_York",
              "utc_offset": "-04:00",
              "sunrise": "2026-10-24T07:55:00-04:00",
              "sunset": "2026-10-24T18:30:00-04:00",
              "astronomical_twilight_begin": "2026-10-24T06:20:00-04:00",
              "astronomical_twilight_end": "2026-10-24T20:05:00-04:00",
              "day_length": 38100,
              "sun_status": "normal",
              "moon_phase": "Waxing Gibbous",
              "moon_illumination": 78.5
            }
            """;

        private static readonly string MORNING_JSON = """
            {
              "date": "2026-10-25",
              "tzid": "America/New_York",
              "utc_offset": "-04:00",
              "sunrise": "2026-10-25T07:56:00-04:00",
              "sunset": "2026-10-25T18:29:00-04:00",
              "astronomical_twilight_begin": "2026-10-25T06:21:00-04:00",
              "astronomical_twilight_end": "2026-10-25T20:04:00-04:00",
              "day_length": 37980,
              "sun_status": "normal",
              "moon_phase": "Waxing Gibbous",
              "moon_illumination": 86.1
            }
            """;

        private DateTime nightOf;

        [SetUp]
        public void Setup()
        {
            nightOf = new DateTime(2026, 10, 24);
        }

        //---ReadTime, the timezone test is the important one
        [Test]
        public void ReadTime_KeepsTheMuseumsClockTime()
        {
            //---19:58 in cleveland should stay 19:58, not turn into irish time
            DateTime time = SunService.ReadTime("2026-10-24T19:58:10-04:00");
            Assert.That(time, Is.EqualTo(new DateTime(2026, 10, 24, 19, 58, 10)));
        }

        //---CreateNightReport puts the evening and the next morning together

        [Test]
        public void CreateNightReport_DarkEndsTheNextMorning()
        {
            NightReport night = SunService.CreateNightReport(nightOf, SunService.ParseSunDay(EVENING_JSON), SunService.ParseSunDay(MORNING_JSON));
            Assert.That(night.DarkEnds, Is.EqualTo(new DateTime(2026, 10, 25, 6, 21, 0)));
        }

        [Test]
        public void CreateNightReport_NeverFullyDark_ThrowsApiException()
        {
            SunDayJSON evening = SunService.ParseSunDay(EVENING_JSON);
            evening.astronomical_twilight_end = null;
            Assert.That(() => SunService.CreateNightReport(nightOf, evening, SunService.ParseSunDay(MORNING_JSON)), Throws.TypeOf<ApiException>());
        }
    }
}