using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SteganoBlaze;
using System.Globalization;
using Microsoft.JSInterop;
using SteganoBlaze.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomStart;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = MudBlazor.Variant.Filled;
    config.SnackbarConfiguration.ClearAfterNavigation = true;
});
builder.Services.AddLocalization();
builder.Services.AddSingleton<AppState>();
builder.Services.AddSingleton(serviceProvider => (IJSInProcessRuntime)serviceProvider.GetRequiredService<IJSRuntime>());

var host = builder.Build();
var js = host.Services.GetRequiredService<IJSRuntime>();

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pl-Pl");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("pl-Pl");

var appState = host.Services.GetRequiredService<AppState>();
appState.webPUnavailable = await js.InvokeAsync<bool>("disableWebP");

await host.RunAsync();