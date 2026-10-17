using HeistPlanner.Models;
using HeistPlanner.Services;
using NUnit.Framework;

namespace HeistPlanner.UnitTests
{
    //---no real API calls here, just sample json in the same shape RandomUser sends back
    public class CrewServiceTests
    {
        private static readonly string SAMPLE_JSON = """
            {
              "results": [
                {
                  "name": { "title": "Ms", "first": "Ana", "last": "Ruiz" },
                  "location": { "country": "Spain" },
                  "dob": { "date": "1995-04-12T10:00:00.000Z", "age": 30 },
                  "picture": { "large": "https://example.com/ana.jpg" },
                  "login": { "uuid": "uuid-ana" }
                },
                {
                  "name": { "title": "Mr", "first": "Tom", "last": "Byrne" },
                  "location": { "country": "Ireland" },
                  "dob": { "date": "1980-01-01T10:00:00.000Z", "age": 45 },
                  "picture": { "large": "https://example.com/tom.jpg" },
                  "login": { "uuid": "uuid-tom" }
                }
              ],
              "info": { "results": 2 }
            }
            """;

        private RandomUserResult ana;

        [SetUp]
        public void Setup()
        {
            ana = CrewService.ParseCandidates(SAMPLE_JSON)[0];
            ana.skill = 80;
        }

        [Test]
        public void ParseCandidates_ValidJson_ReturnsEveryCandidate()
        {
            List<RandomUserResult> candidates = CrewService.ParseCandidates(SAMPLE_JSON);
            Assert.That(candidates, Has.Exactly(2).Items);
        }

        [Test]
        public void ParseCandidates_ValidJson_ReadsNameAndCountry()
        {
            List<RandomUserResult> candidates = CrewService.ParseCandidates(SAMPLE_JSON);
            Assert.That(candidates[1].name.first, Is.EqualTo("Tom"));
            Assert.That(candidates[1].name.last, Is.EqualTo("Byrne"));
            Assert.That(candidates[1].location.country, Is.EqualTo("Ireland"));
        }

        [Test]
        public void ParseCandidates_ValidJson_ReadsAgePhotoAndId()
        {
            List<RandomUserResult> candidates = CrewService.ParseCandidates(SAMPLE_JSON);
            Assert.That(candidates[1].dob.age, Is.EqualTo(45));
            Assert.That(candidates[1].picture.large, Is.EqualTo("https://example.com/tom.jpg"));
            Assert.That(candidates[1].login.uuid, Is.EqualTo("uuid-tom"));
        }

        [Test]
        public void ParseCandidates_SkillIsNotInJson_StartsAtZero()
        {
            //---proves skill doesnt come from the API, GetCandidates gives it out after parsing
            List<RandomUserResult> candidates = CrewService.ParseCandidates(SAMPLE_JSON);
            Assert.That(candidates[0].skill, Is.EqualTo(0));
        }

        [Test]
        public void ParseCandidates_NoResults_ReturnsEmptyList()
        {
            List<RandomUserResult> candidates = CrewService.ParseCandidates("""{ "results": [] }""");
            Assert.That(candidates, Is.Empty);
        }

        [Test]
        public void ParseCandidates_NullJson_ReturnsEmptyList()
        {
            List<RandomUserResult> candidates = CrewService.ParseCandidates("null");
            Assert.That(candidates, Is.Empty);
        }

        [Test]
        public void ParseCandidates_BrokenJson_ThrowsApiException()
        {
            Assert.That(() => CrewService.ParseCandidates("{ results: [ oops"), Throws.TypeOf<ApiException>());
        }

        //---CreateCrewMember gives back the right subclass for each role
        [Test]
        public void CreateCrewMember_Hacker_ReturnsHacker()
        {
            Assert.That(CrewService.CreateCrewMember(ana, CrewRole.Hacker), Is.TypeOf<Hacker>());
        }

        [Test]
        public void CreateCrewMember_Safecracker_ReturnsSafecracker()
        {
            Assert.That(CrewService.CreateCrewMember(ana, CrewRole.Safecracker), Is.TypeOf<Safecracker>());
        }

        [Test]
        public void CreateCrewMember_Lookout_ReturnsLookout()
        {
            Assert.That(CrewService.CreateCrewMember(ana, CrewRole.Lookout), Is.TypeOf<Lookout>());
        }

        [Test]
        public void CreateCrewMember_Driver_ReturnsDriver()
        {
            Assert.That(CrewService.CreateCrewMember(ana, CrewRole.Driver), Is.TypeOf<Driver>());
        }

        [Test]
        public void CreateCrewMember_InsideMan_ReturnsInsideMan()
        {
            Assert.That(CrewService.CreateCrewMember(ana, CrewRole.InsideMan), Is.TypeOf<InsideMan>());
        }

        [Test]
        public void CreateCrewMember_CopiesPersonDetails()
        {
            CrewMember member = CrewService.CreateCrewMember(ana, CrewRole.Driver);
            Assert.That(member.Id, Is.EqualTo("uuid-ana"));
            Assert.That(member.Name, Is.EqualTo("Ana Ruiz"));
            Assert.That(member.Country, Is.EqualTo("Spain"));
            Assert.That(member.Skill, Is.EqualTo(80));
        }

        [Test]
        public void CreateCrewMember_SkillOver100_IsCapped()
        {
            ana.skill = 150;
            CrewMember member = CrewService.CreateCrewMember(ana, CrewRole.Driver);
            Assert.That(member.Skill, Is.EqualTo(100));
        }

        [Test]
        public void CreateCrewMember_UnknownRole_ThrowsArgumentException()
        {
            //----ref casting a number to an enum, 99 isnt one of our 5 roles so it hits the default in the switch https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum
            CrewRole fakeRole = (CrewRole)99;
            Assert.That(() => CrewService.CreateCrewMember(ana, fakeRole), Throws.TypeOf<ArgumentException>());
        }
    }
}