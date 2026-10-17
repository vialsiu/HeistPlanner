using HeistPlanner.Models;
using Newtonsoft.Json;
using RestSharp;

namespace HeistPlanner.Services
{
    //---gets random people from the RandomUser API to hire as crew
    //----ref RandomUser API docs (results = how many people, inc = only the fields we want)  https://randomuser.me/documentation
    public class CrewService
    {
        public static readonly string RANDOM_USER_URL = "https://randomuser.me/api/";
        public static readonly string RANDOM_USER_FIELDS = "name,location,dob,picture,login";

        private static Random generator = new Random();

        public static List<RandomUserResult> GetCandidates(int count)
        {
            var client = new RestClient(RANDOM_USER_URL);
            var request = new RestRequest();

            request.AddParameter("results", count);
            request.AddParameter("inc", RANDOM_USER_FIELDS);
            var response = client.Execute(request);

            //----ref RestSharp IsSuccessful https://restsharp.dev/docs/usage/response
            //----ref HttpStatusCode cast to int to show the number https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                throw new ApiException($"Couldn't reach the candidate list (status {(int)response.StatusCode}). Try again later.");
            }

            List<RandomUserResult> candidates = ParseCandidates(response.Content);

            //---the API doesnt have a skill rating so we make one up for each person, 40 to 95
            foreach (RandomUserResult person in candidates)
            {
                person.skill = generator.Next(40, 96);
            }

            return candidates;
        }

        //---separate from the API call so it can be unit tested with sample json
        public static List<RandomUserResult> ParseCandidates(string receivedJSON)
        {
            try
            {
                RandomUserJSON? userResponse = JsonConvert.DeserializeObject<RandomUserJSON>(receivedJSON);
                if (userResponse != null && userResponse.results != null)
                {
                    return userResponse.results;
                }
                return new List<RandomUserResult>();
            }
            //----ref Newtonsoft JsonException https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonException.htm
            catch (JsonException)
            {
                throw new ApiException("The candidate data came back in an unexpected format.");
            }
        }

        //---makes the right kind of crew member for the role picked on the page
        public static CrewMember CreateCrewMember(RandomUserResult person, CrewRole role)
        {
            string id = person.login.uuid;
            string name = $"{person.name.first} {person.name.last}";
            string country = person.location.country;
            int age = person.dob.age;
            string photoUrl = person.picture.large;

            switch (role)
            {
                case CrewRole.Hacker:
                    return new Hacker(id, name, country, age, photoUrl, person.skill);
                case CrewRole.Safecracker:
                    return new Safecracker(id, name, country, age, photoUrl, person.skill);
                case CrewRole.Lookout:
                    return new Lookout(id, name, country, age, photoUrl, person.skill);
                case CrewRole.Driver:
                    return new Driver(id, name, country, age, photoUrl, person.skill);
                case CrewRole.InsideMan:
                    return new InsideMan(id, name, country, age, photoUrl, person.skill);
                default:
                    //----ref ArgumentException for when a method gets a value it cant use https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception
                    throw new ArgumentException($"Unknown crew role: {role}");
            }
        }
    }

    //---classes matching the RandomUser json, lowercase names so they match the json like PurgoMalumJSON
    public class RandomUserJSON
    {
        public List<RandomUserResult> results { get; set; } = new List<RandomUserResult>();
    }

    public class RandomUserResult
    {
        public RandomUserName name { get; set; } = new RandomUserName();
        public RandomUserLocation location { get; set; } = new RandomUserLocation();
        public RandomUserDob dob { get; set; } = new RandomUserDob();
        public RandomUserPicture picture { get; set; } = new RandomUserPicture();
        public RandomUserLogin login { get; set; } = new RandomUserLogin();

        //---not from the API, we give everyone a skill ourselves in GetCandidates
        public int skill { get; set; }
    }

    public class RandomUserName
    {
        public string first { get; set; } = "";
        public string last { get; set; } = "";
    }

    public class RandomUserLocation
    {
        public string country { get; set; } = "";
    }

    public class RandomUserDob
    {
        public int age { get; set; }
    }

    public class RandomUserPicture
    {
        public string large { get; set; } = "";
    }

    public class RandomUserLogin
    {
        public string uuid { get; set; } = "";
    }
}