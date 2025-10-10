using Molit_IN.Library.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Molit_IN.Client.Services
{
    public class AuthorizationLevelTemplateServices
    {
        public List<AuthorizationLevelTemplateCLS> lista;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;
        public event Func<Task> OnChange;
        public event Func<int, Task> OnEdit;
        public event Func<AuthorizationLevelTemplateCLS, Task> OnSearch;
        public AuthorizationLevelTemplateServices(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            lista = new List<AuthorizationLevelTemplateCLS>();
            _tokenService = tokenService;
        }

        // Este método garantiza que _httpClient tenga siempre el Bearer antes de la petición.
        private async Task EnsureAuthorizationAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token) && token != "No token found!")
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<AuthorizationLevelTemplateCLS>> listar()
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.GetFromJsonAsync<List<AuthorizationLevelTemplateCLS>>("api/AuthorizationLevelTemplate");
                if (response != null)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {

                return lista;
            }

            return lista;
        }

     

        public async Task<AuthorizationLevelTemplateCLS> recuperar(int id)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.GetFromJsonAsync<AuthorizationLevelTemplateCLS>("api/AuthorizationLevelTemplate/" + id);
                if (response != null)
                {
                    return response;
                }
                return new AuthorizationLevelTemplateCLS();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<bool> agregar(AuthorizationLevelTemplateCLS oAuthorizationLevelTemplateCLS)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.PostAsJsonAsync("api/AuthorizationLevelTemplate", oAuthorizationLevelTemplateCLS);
                if (response.IsSuccessStatusCode)
                {
                    notificarCambios();
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }

            return false;

        }

        public async Task<bool> editar(AuthorizationLevelTemplateCLS oAuthorizationLevelTemplateCLS)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.PutAsJsonAsync("api/AuthorizationLevelTemplate", oAuthorizationLevelTemplateCLS);
                if (response.IsSuccessStatusCode)
                {
                    notificarCambios();
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
            return false;

        }

        public async Task<bool> eliminar(int id)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.DeleteAsync("api/AuthorizationLevelTemplate/" + id);
                if (response.IsSuccessStatusCode)
                {
                    notificarCambios();
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }

            return false;
        }


        public void notificarCambios()
        {
            OnChange?.Invoke();
        }

        public void notificarEdit(int idcarrera)
        {
            OnEdit?.Invoke(idcarrera);
        }

        public void notificarSearch(AuthorizationLevelTemplateCLS oAuthorizationLevelTemplateCLS)
        {
            OnSearch?.Invoke(oAuthorizationLevelTemplateCLS);
        }
    }
}
