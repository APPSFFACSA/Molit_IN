using Molit_IN.Library.User;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Molit_IN.Client.Services
{
    public class UserServices
    {
        public List<UserListCLS> lista;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;

        // Event bus
        public event Func<Task>? OnChange;
        public event Func<int, Task>? OnEdit;
        public event Func<UserListCLS, Task>? OnSearch;
        public event Func<string, Task>? OnSerach2;

        public UserServices(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            lista = new List<UserListCLS>();
            _tokenService = tokenService;
        }

        private async Task EnsureAuthorizationAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token) && token != "No token found!")
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<UserListCLS>> listarUsuario()
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.GetFromJsonAsync<List<UserListCLS>>("api/User");
            return response ?? lista;
        }

        public async Task<List<UserListCLS>> buscarUsuario(string texto)
        {
            var lista = await listarUsuario();
            if (string.IsNullOrWhiteSpace(texto)) return lista;

            var term = texto.ToUpperInvariant();
            return lista.Where(p => (p?.UserName ?? string.Empty).ToUpperInvariant().Contains(term)).ToList();
        }

        public async Task<UserFormAddCLS> recuperarUsuario(int idusuario)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.GetFromJsonAsync<UserFormAddCLS>("api/User/" + idusuario);
            return response ?? new UserFormAddCLS();
        }

        public async Task<bool> agregar(UserFormAddCLS oUsuarioFormAddCLS)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.PostAsJsonAsync("api/User", oUsuarioFormAddCLS);
            if (response.IsSuccessStatusCode)
            {
                notificarCambios();
                return true;
            }
            return false;
        }

        public async Task<bool> eliminar(int idusuario)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.DeleteAsync("api/User/" + idusuario);
            if (response.IsSuccessStatusCode)
            {
                notificarCambios();
                return true;
            }
            return false;
        }

        public async Task<bool> actualizar(UserFormAddCLS u)
        {
            if (u is null || u.IdUser <= 0) return false;

            await EnsureAuthorizationAsync();
            var resp = await _httpClient.PutAsJsonAsync($"api/User/{u.IdUser}", u);
            if (resp.IsSuccessStatusCode)
            {
                notificarCambios();
                return true;
            }
            return false;
        }

        // ==== Notificaciones de eventos (fire-and-forget para no bloquear UI) ====
        public void notificarCambios()
        {
            var handler = OnChange;
            if (handler is not null) _ = handler.Invoke();
        }

        public void notificarEdit(int idUsuario)
        {
            var handler = OnEdit;
            if (handler is not null) _ = handler.Invoke(idUsuario);
        }

        public void notificarSearch(UserListCLS oUsuarioListCLS)
        {
            var handler = OnSearch;
            if (handler is not null) _ = handler.Invoke(oUsuarioListCLS);
        }

        public void notificarSearch2(string nombreusuario)
        {
            var handler = OnSerach2;
            if (handler is not null) _ = handler.Invoke(nombreusuario);
        }
    }
}
