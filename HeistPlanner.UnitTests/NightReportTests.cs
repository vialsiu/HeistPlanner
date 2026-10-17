using HeistPlanner.Models;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    public class NightReportTests
    {
        private NightReport night;

        //-----dark from 20:05 on the 24th until 06:21 on the 25th, moon 78.5%
        [SetUp]
        public void Setup()
        {
            night = new NightReport(
                new DateTime(2026, 10, 24),
                new DateTime(2026, 10, 24, 18, 30, 0),
                new DateTime(2026, 10, 24, 20, 5, 0),
                new DateTime(2026, 10, 25, 6, 21, 0),
                new DateTime(2026, 10, 25, 7, 56, 0),
                "Waxing Gibbous",
                78.5);
        }

        //---IsDarkAt, boundary testing on both ends of the dark window
        [Test]
        public void IsDarkAt_Midnight_IsTrue()
        {
            Assert.That(night.IsDarkAt(new DateTime(2026, 10, 25, 0, 0, 0)), Is.True);
        }

        [Test]
        public void IsDarkAt_ExactlyWhenDarkStarts_IsTrue()
        {
            Assert.That(night.IsDarkAt(new DateTime(2026, 10, 24, 20, 5, 0)), Is.True);
        }

        [Test]
        public void IsDarkAt_OneMinuteBeforeDark_IsFalse()
        {
            Assert.That(night.IsDarkAt(new DateTime(2026, 10, 24, 20, 4, 0)), Is.False);
        }

        [Test]
        public void IsDarkAt_ExactlyWhenDarkEnds_IsTrue()
        {
            Assert.That(night.IsDarkAt(new DateTime(2026, 10, 25, 6, 21, 0)), Is.True);
        }

        [Test]
        public void IsDarkAt_OneMinuteAfterDarkEnds_IsFalse()
        {
            Assert.That(night.IsDarkAt(new DateTime(2026, 10, 25, 6, 22, 0)), Is.False);
        }

        //---GetDarkHours, 20:05 to 06:21 is 10 hours 16 minutes = 10.27, rounded to 10.3
        [Test]
        public void GetDarkHours_RoundsToOneDecimal()
        {
            Assert.That(night.GetDarkHours(), Is.EqualTo(10.3));
        }

        //---IsBrightMoon, boundary at 50%
        [Test]
        public void IsBrightMoon_ExactlyFiftyPercent_IsTrue()
        {
            night.MoonIllumination = 50;
            Assert.That(night.IsBrightMoon(), Is.True);
        }

        [Test]
        public void IsBrightMoon_JustUnderFifty_IsFalse()
        {
            night.MoonIllumination = 49.9;
            Assert.That(night.IsBrightMoon(), Is.False);
        }

        //----GetGoTime, hours before midday belong to the next day
        [Test]
        public void GetGoTime_TenPm_IsSameDay()
        {
            DateTime goTime = NightReport.GetGoTime(new DateTime(2026, 10, 24), 22);
            Assert.That(goTime, Is.EqualTo(new DateTime(2026, 10, 24, 22, 0, 0)));
        }

        [Test]
        public void GetGoTime_TwoAm_IsNextDay()
        {
            DateTime goTime = NightReport.GetGoTime(new DateTime(2026, 10, 24), 2);
            Assert.That(goTime, Is.EqualTo(new DateTime(2026, 10, 25, 2, 0, 0)));
        }

        [Test]
        public void ToString_ContainsTheMoonPhase()
        {
            Assert.That(night.ToString(), Does.Contain("Waxing Gibbous"));
        }
    }
}