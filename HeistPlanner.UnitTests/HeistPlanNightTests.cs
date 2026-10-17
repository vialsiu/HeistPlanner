using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    //---tests for the night part of HeistPlan (targets and crew are tested in their own files)
    public class HeistPlanNightTests
    {
        private HeistPlan plan;
        private NightReport night;
        private WeatherReport weather;

        [SetUp]
        public void Setup()
        {
            plan = new HeistPlan();
            night = new NightReport(
                new DateTime(2026, 10, 30),
                new DateTime(2026, 10, 30, 18, 23, 0),
                new DateTime(2026, 10, 30, 19, 57, 0),
                new DateTime(2026, 10, 31, 6, 23, 0),
                new DateTime(2026, 10, 31, 7, 57, 0),
                "Waning Gibbous",
                74);
            weather = new WeatherReport(new DateTime(2026, 10, 31, 4, 0, 0), 18, 19, 100, 24100, 20, 3);
        }

        [Test]
        public void NewPlan_NoNightChecked_GoTimeIsNull()
        {
            Assert.That(plan.GoTime, Is.Null);
        }

        [Test]
        public void SetNight_StoresTheNightAndWeather()
        {
            plan.SetNight(night, weather, 4);
            Assert.That(plan.Night, Is.EqualTo(night));
            Assert.That(plan.Weather, Is.EqualTo(weather));
        }

        [Test]
        public void SetNight_FourAm_GoTimeIsTheNextDay()
        {
            plan.SetNight(night, weather, 4);
            Assert.That(plan.GoTime, Is.EqualTo(new DateTime(2026, 10, 31, 4, 0, 0)));
        }
    }
}