using DialMock.Components;
using DialMock.Core.Engine;
using DialMock.Core.Services;
using DialMock.Rendering;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<DialRuleEngine>();
builder.Services.AddScoped<DialEngine>();
builder.Services.AddScoped<SvgDialRenderer>();


// Force loading of static assets in Production
builder.WebHost.UseStaticWebAssets(); 
 


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapStaticAssets();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();