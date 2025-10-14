// Client/Program.cs
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Molit_IN.Client;
using Molit_IN.Client.Provider;   // Authentication (tu AuthStateProvider)
using Molit_IN.Client.Services;
using BlazorBootstrap;
using CurrieTechnologies.Razor.SweetAlert2;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// SweetAlert2 (si lo usas en componentes)
builder.Services.AddSweetAlert2();

// === Base de tu API ===
var apiBase = new Uri("https://localhost:44304/"); // ajusta si cambia

// === Auth / State ===
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, Authentication>();
builder.Services.AddScoped<Authentication>(sp => (Authentication)sp.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddScoped<TokenService>();

// === Handlers ===
builder.Services.AddTransient<AuthMessageHandler>();
builder.Services.AddTransient<LoggingHandler>(); // opcional para ver 401/403/404 en consola

// === HttpClient genérico (si algún componente lo usa directo) ===
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = apiBase });

builder.Services.AddHttpClient<BranchServices>(c => c.BaseAddress = apiBase)
    .AddHttpMessageHandler<AuthMessageHandler>()
    .AddHttpMessageHandler<LoggingHandler>();

builder.Services.AddHttpClient<AuthorizationServices>(c => c.BaseAddress = apiBase)
    .AddHttpMessageHandler<AuthMessageHandler>()
    .AddHttpMessageHandler<LoggingHandler>();

builder.Services.AddHttpClient<AuthorizationLevelTemplateServices>(c => c.BaseAddress = apiBase)
    .AddHttpMessageHandler<AuthMessageHandler>()
    .AddHttpMessageHandler<LoggingHandler>();

//UserServices como Scoped (misma instancia para FormUser y ListUser)
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<RoleServices>();
builder.Services.AddScoped<PilotServices>();
builder.Services.AddScoped<IYourImageService, YourImageService>();

// === Otros servicios locales === 
builder.Services.AddScoped<LoginServices>();

// === UI libs ===
builder.Services.AddBlazorBootstrap();

await builder.Build().RunAsync();


// ===== Handlers =====
public class AuthMessageHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;
    public AuthMessageHandler(TokenService tokenService) => _tokenService = tokenService;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = await _tokenService.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token) && token != "No token found!")
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, ct);
    }
}

public class LoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var resp = await base.SendAsync(request, ct);
        if (!resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"HTTP {(int)resp.StatusCode} {resp.StatusCode} {request.Method} {request.RequestUri}");
            var body = await resp.Content.ReadAsStringAsync(ct);
            Console.WriteLine(body);
        }
        return resp;
    }
}
