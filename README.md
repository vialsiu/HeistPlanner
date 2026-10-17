# The Heist Planner

A Blazor web app for planning a (fictional) art heist. Scout real artworks,
recruit a crew, check the darkness and weather on the night, then run the job.

SD4 Service Oriented Architecture: CA1, DkIT 2026/27
D00265675
## Web services used
- [Art Institute of Chicago API](https://api.artic.edu/docs/): artworks and images
- [RandomUser](https://randomuser.me/): crew candidates
- [Sunrise-Sunset](https://sunrise-sunset.org/api): darkness window
- [Open-Meteo](https://open-meteo.com/): weather on the night


## Testing
Unit tests are in the `HeistPlanner.UnitTests` project (NUnit).
Run them with **Tests > Run All Tests** in Rider, or `dotnet test` in the project folder.

- Tests never call the real web services. Each service has a separate `Parse...` method
  that is tested with sample JSON so the tests still pass if an API is down.
- Each test checks one thing and `[SetUp]` creates fresh objects before every test
  so tests don't affect each other.

## References
will be added as the project grows

### Web services
- [Cleveland Museum of Art Open Access API](https://openaccess-api.clevelandart.org/) – artworks and images (switched from the Art Institute of Chicago API after its image server began blocking embedded images)
- [RestSharp: Handling responses (IsSuccessful)](https://restsharp.dev/docs/usage/response)

### C# and .NET
- [Microsoft: Guidelines for overriding Equals() and operator ==](https://learn.microsoft.com/en-us/previous-versions/ms173147(v=vs.90))
- [Microsoft: How to create user-defined exceptions](https://learn.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions)
- [Microsoft: HttpStatusCode enum](https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode)
- [Microsoft: Raw string literals](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/raw-string)
- [Microsoft: Exclude files from the build (MSBuild)](https://learn.microsoft.com/en-us/visualstudio/msbuild/how-to-exclude-files-from-the-build)

### Blazor
- [Microsoft: Blazor event handling (@onerror on images)](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/event-handling)

### Libraries
- [Newtonsoft.Json: JsonProperty attribute](https://www.newtonsoft.com/json/help/html/JsonPropertyName.htm)
- [Newtonsoft.Json: JsonException](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonException.htm)

### Testing
- [NUnit: Constraint model (Assert.That)](https://docs.nunit.org/articles/nunit/writing-tests/assertions/assertion-models/constraint.html)

