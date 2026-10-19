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
        public void IsDarkAt_ExactlyWhenDarkStarts_IsTrue()
        {
            Assert.That(night.IsDarkAt(new DateTime(2026, 10, 24, 20, 5, 0)), Is.True);
        }

        [Test]
        public void IsDarkAt_OneMinuteBeforeDark_IsFalse()
        {
            Assert.That(night.IsDarkAt(new DateTime(2026, 10, 24, 20, 4, 0)), Is.False);
        }

        //----GetGoTime, hours before midday belong to the next day

        [Test]
        public void GetGoTime_TwoAm_IsNextDay()
        {
            DateTime goTime = NightReport.GetGoTime(new DateTime(2026, 10, 24), 2);
            Assert.That(goTime, Is.EqualTo(new DateTime(2026, 10, 25, 2, 0, 0)));
        }

    }
}