using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Molit_IN.Library.Login;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Molit_IN.Client.Provider
{
    public class Authentication : AuthenticationStateProvider
    {
        private ClaimsPrincipal _user =
                new ClaimsPrincipal(new ClaimsIdentity());
        private readonly IJSRuntime _js;

        public Authentication(IJSRuntime js)
        {
            _js = js;
        }


        /// <summary>
        /// Se llama al hacer login: guarda token y dispara cambio de estado.
        /// </summary>
        public async Task Entrar(string token)
        {
            // 1) Guardamos el token (aunque ya lo guardó el servicio)
            await _js.InvokeVoidAsync(
                "localStorage.setItem", "authToken", token);

            // 2) Parseamos claims del JWT
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");

            _user = new ClaimsPrincipal(identity);

            // 3) Notificamos a Blazor que cambió el usuario
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(_user)));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Leer token desde localStorage
            var token = await _js.InvokeAsync<string>(
                "localStorage.getItem", "authToken");
            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity()));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");

            return new AuthenticationState(
                new ClaimsPrincipal(identity));
        }

        public async Task CerrarSesion()
        {
            await _js.InvokeVoidAsync(
                "localStorage.removeItem", "authToken");
            _user = new ClaimsPrincipal(
                new ClaimsIdentity());
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(_user)));
        }


        //public Authentication(IJSRuntime jsRuntime)
        //{
        //    _jsRuntime = jsRuntime;
        //    user = new ClaimsPrincipal(new ClaimsIdentity());
        //    Task.Run(async () =>
        //    {
        //        var userData = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "userData")
        //         .ConfigureAwait(false);
        //        if (string.IsNullOrEmpty(userData))
        //        {
        //            user = JsonConvert.DeserializeObject<ClaimsPrincipal>(userData);
        //        }
        //        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));

        //    });
        //}
        //public override Task<AuthenticationState> GetAuthenticationStateAsync()
        //{

        //    // user = new ClaimsPrincipal();
        //    return Task.FromResult(new AuthenticationState(user));
        //    //var user = new ClaimsPrincipal();
        //    //return Task.FromResult(new AuthenticationState(user));  
        //}

        //public async void entrar(UserLoginCLS oUserLoginCLS)
        //{
        //    string objeto = JsonConvert.SerializeObject(oUserLoginCLS);
        //    var identity = new ClaimsIdentity(new[]
        //   {
        //        new Claim(ClaimTypes.Name, objeto)
        //    }, "auth");
        //    user = new ClaimsPrincipal(identity);
        //    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        //    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "userData",
        //        JsonConvert.SerializeObject(oUserLoginCLS));
        //    // return Task.FromResult(new AuthenticationState(user));
        //}

        //public async void cerrarsesion()
        //{
        //    user = new ClaimsPrincipal();
        //    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        //    await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "userData");

        //}

        //public async Task<UserLoginCLS> usuerioLogueado()
        //{
        //    if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
        //    {
        //        var userNameClaim = user.FindFirst(ClaimTypes.Name);
        //        if (userNameClaim != null)
        //        {
        //            if (userNameClaim.Value != null)
        //            {
        //                return JsonConvert.DeserializeObject<UserLoginCLS>(userNameClaim.Value);
        //            }

        //        }


        //    }
        //    var userData = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "userData");
        //    if (!string.IsNullOrEmpty(userData))
        //    {
        //        var userLogin = JsonConvert.DeserializeObject<UserLoginCLS>(userData);
        //        string objeto = JsonConvert.SerializeObject(userLogin);
        //        var identity = new ClaimsIdentity(new[]
        //       {
        //        new Claim(ClaimTypes.Name,objeto)
        //    }, "auth");
        //        user = new ClaimsPrincipal(identity);
        //        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        //        return userLogin;
        //    }
        //    return null;
        //}
    }
}
