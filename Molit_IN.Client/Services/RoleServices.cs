using Molit_IN.Library.Rol;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Molit_IN.Client.Services
{
    public class RoleServices
    {
        public List<RolListCLS> lista;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;

        public event Func<Task>? OnChange;
        public event Func<int, Task>? OnEdit;
        public event Func<RolListCLS, Task>? OnSearch;
        public event Func<string, Task>? OnSearch2;

        public RoleServices(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            lista = new List<RolListCLS>();
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

        public async Task<List<RolListCLS>> listarRole()
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.GetFromJsonAsync<List<RolListCLS>>("api/Rol");
            return response ?? lista;
        }

        public async Task<List<RolListCLS>> buscarRole(string texto)
        {
            var lista = await listarRole();
            if (string.IsNullOrWhiteSpace(texto)) return lista;
            var term = texto.ToUpperInvariant();
            return lista.Where(p => (p?.RoleName ?? string.Empty).ToUpperInvariant().Contains(term)).ToList();
        }

        public async Task<RolFormAdd> recuperarRole(int id)
        {
            await EnsureAuthorizationAsync();
            return await _httpClient.GetFromJsonAsync<RolFormAdd>($"api/Rol/{id}") ?? new RolFormAdd();
        }

        public async Task<bool> agregar(RolFormAdd model)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.PostAsJsonAsync("api/Rol", model);
            if (response.IsSuccessStatusCode) { notificarCambios(); return true; }
            return false;
        }

        public async Task<bool> editar(RolFormAdd model)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.PutAsJsonAsync("api/Rol", model); //
            if (response.IsSuccessStatusCode) { notificarCambios(); return true; }
            return false;
        }

        public async Task<bool> eliminar(int id)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.DeleteAsync($"api/Rol/{id}");
            if (response.IsSuccessStatusCode) { notificarCambios(); return true; }
            return false;
        }

        public void notificarCambios()
        {
            var h = OnChange; if (h is not null) _ = h.Invoke();
        }
        public void notificarEdit(int id)
        {
            var h = OnEdit; if (h is not null) _ = h.Invoke(id);
        }
        public void notificarSearch(RolListCLS r)
        {
            var h = OnSearch; if (h is not null) _ = h.Invoke(r);
        }
        public void notificarSearch2(string q)
        {
            var h = OnSearch2; if (h is not null) _ = h.Invoke(q);
        }
    }
}
