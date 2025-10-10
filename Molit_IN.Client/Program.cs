using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Molit_IN.Client;
using Molit_IN.Client.Provider;
using Molit_IN.Client.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorBootstrap;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient -> **API base URL**
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:44304/")
});

// Servicios propios que usas
builder.Services.AddScoped<LoginServices>();
builder.Services.AddScoped<AuthorizationLevelTemplateServices>();
builder.Services.AddScoped<AuthorizationServices>();
builder.Services.AddScoped<TokenService>();

// 👇 REGISTRA RoleServices AQUÍ
builder.Services.AddScoped<RoleServices>();
// (Si tuvieras interfaz: builder.Services.AddScoped<IRoleServices, RoleServices>();)

// Autorización y AuthStateProvider personalizado
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, Authentication>();
builder.Services.AddScoped<Authentication>(sp => (Authentication)sp.GetRequiredService<AuthenticationStateProvider>());

// UI libs
builder.Services.AddBlazorBootstrap();
builder.Services.AddSweetAlert2();

await builder.Build().RunAsync();
