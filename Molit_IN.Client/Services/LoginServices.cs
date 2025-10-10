using Microsoft.JSInterop;
using Molit_IN.Library.Login;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Molit_IN.Client.Services
{
    public class LoginServices
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;
        public LoginServices(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        /// <summary>
        /// Llama al login y devuelve el JWT.
        /// </summary>z|
        public async Task<string?> login(LoginCLS creds)
        {
            var resp = await _httpClient.PostAsJsonAsync("api/Login", creds);
            if (!resp.IsSuccessStatusCode)
                return null;

            var jwtResp = await resp.Content
                .ReadFromJsonAsync<JwtSettings>();
            if (jwtResp?.Token != null)
            {
                // guarda en localStorage
                await _js.InvokeVoidAsync(
                    "localStorage.setItem",
                    "authToken",
                    jwtResp.Token);
                return jwtResp.Token;
            }
            return null;
        }



    }
}
