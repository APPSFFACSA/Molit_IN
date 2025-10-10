using Molit_IN.Library.Authorization;
using Molit_IN.Library.Menu;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Molit_IN.Client.Services
{
    public class AuthorizationServices
    {
        public List<AuthorizationCLS> lista;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;
        public event Func<Task> OnChange;
        public event Func<int, Task> OnEdit;
        public event Func<AuthorizationCLS, Task> OnSearch;
        public AuthorizationServices(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            lista = new List<AuthorizationCLS>();
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

        public async Task<List<AuthorizationCLS>> listar()
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.GetFromJsonAsync<List<AuthorizationCLS>>("api/Authorization/");
                if (response != null)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                lista.Add(new AuthorizationCLS()
                {
                    IdAuthorization = 0,
                    LevelAutho = 0,
                    IdUser = 0,
                    Status = "",
                    DateRevision = DateTime.Now,
                    Comments = ex.Message,
                    IdSettlement = 0
                });

                return lista;
            }

            return lista;
        }

        //recupera todas las autorizaciones del usuario.
        public async Task<List<AuthorizationCLS>> listarPorUsuario(int id)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.GetFromJsonAsync<List<AuthorizationCLS>>("api/Authorization/getbyuser/" + id);
                if (response != null)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                lista.Add(new AuthorizationCLS()
                {
                    IdAuthorization = 0,
                    LevelAutho = 0,
                    IdUser = 0,
                    Status = "",
                    DateRevision = DateTime.Now,
                    Comments = ex.Message,
                    IdSettlement = 0
                });
               
                return lista;
            }

            return lista;
        }

        //recupera por id de authroizacion.
        public async Task<AuthorizationCLS> recuperar(int id)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.GetFromJsonAsync<AuthorizationCLS>("api/Authorization/" + id);
                if (response != null)
                {
                    return response;
                }
                return new AuthorizationCLS();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        //envia a autorizar.
        public async Task<bool> enviarautorizar(AuthorizationCLS oAuthorizationCLS)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.PostAsJsonAsync("api/Authorization/sendforauthorization", oAuthorizationCLS);
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
        //actualiza su estado de autorizacion
        public async Task<bool> AutStatus(AuthorizationCLS oAuthorizationCLS)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.PutAsJsonAsync("api/Authorization/statusAutho", oAuthorizationCLS);
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
            // listacarrera.Add(oCarreraCLS);

        }

        public void notificarCambios()
        {
            OnChange?.Invoke();
        }

        public void notificarEdit(int idcarrera)
        {
            OnEdit?.Invoke(idcarrera);
        }

        public void notificarSearch(AuthorizationCLS oAuthorizationCLS)
        {
            OnSearch?.Invoke(oAuthorizationCLS);
        }


    }
}
