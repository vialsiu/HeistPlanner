using HeistPlanner.Components;
using HeistPlanner.Models;

var builder = WebApplication.CreateBuilder(args);

//---add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//---blazor dependency injection, scoped services and @inject https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection
//-----each user's browser tab gets 1 HeistPlan
builder.Services.AddScoped<HeistPlan>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();