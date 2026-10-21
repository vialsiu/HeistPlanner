# The Heist Planner
 
<img width="1895" height="861" alt="heistplan" src="https://github.com/user-attachments/assets/adf212cc-a64a-4de4-9a8a-e6f7346665ad" />

- A Blazor web app for planning a (fictional) art heist. 

- Scout real artworks,
recruit a crew, check the darkness and weather on the night, then run the job.

- SD4 Service Oriented Architecture: CA1, DkIT
D00265675
## Web services used
- [Cleveland Museum of Art Open Access API](https://openaccess-api.clevelandart.org/): artworks and images
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

### Web services
- [Cleveland Museum of Art Open Access API](https://openaccess-api.clevelandart.org/): artworks and images
- [RestSharp: Handling responses (IsSuccessful)](https://restsharp.dev/docs/usage/response)

### C# and .NET
- [Microsoft: Guidelines for overriding Equals() and operator ==](https://learn.microsoft.com/en-us/previous-versions/ms173147(v=vs.90))
- [Microsoft: How to create user-defined exceptions](https://learn.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions)
- [Microsoft: HttpStatusCode enum](https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode)
- [Microsoft: Raw string literals](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/raw-string)
- [Microsoft: Exclude files from the build (MSBuild)](https://learn.microsoft.com/en-us/visualstudio/msbuild/how-to-exclude-files-from-the-build)
- [Microsoft: DateTimeOffset.Parse](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.parse)
- [Microsoft: CultureInfo.InvariantCulture](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture)
- [Microsoft: Dictionary class](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)
- [Microsoft: virtual keyword](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/virtual)

### Blazor
- [Microsoft: Blazor event handling (@onerror on images)](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/event-handling)
- [Microsoft: Blazor dependency injection (AddScoped and @inject)](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection)
- [Microsoft: Component parameters and RenderFragment (PageHero)](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/#component-parameters)
- [Microsoft: CSS isolation and ::deep](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/css-isolation#child-component-support)
- [Microsoft: Navigation and LocationChanged](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/navigation#handleprevent-location-changes)
- [Microsoft: Call JavaScript from .NET](https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/call-javascript-from-dotnet)

### Libraries
- [Newtonsoft.Json: JsonProperty attribute](https://www.newtonsoft.com/json/help/html/JsonPropertyName.htm)
- [Newtonsoft.Json: JsonException](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonException.htm)
- [Newtonsoft.Json: DateParseHandling](https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_JsonSerializerSettings_DateParseHandling.htm)

### Testing
- [NUnit: Constraint model (Assert.That)](https://docs.nunit.org/articles/nunit/writing-tests/assertions/assertion-models/constraint.html)

### CSS
- [CSS-Tricks: Full width containers in limited width parents](https://css-tricks.com/full-width-containers-limited-width-parents/)
- [MDN: prefers-reduced-motion](https://developer.mozilla.org/en-US/docs/Web/CSS/@media/prefers-reduced-motion)

### Paintings in `wwwroot/images`:
- [Reddit r/TheFrame – Over 400 4K open-source artworks from around the world](https://www.reddit.com/r/TheFrame/comments/10cu8hg/over_400_4k_opensource_artworks_from_around_the/)
