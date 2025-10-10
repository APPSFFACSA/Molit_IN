using Molit_IN.Client.Services;
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
        public event Func<Task> OnChange;
        public event Func<int, Task> OnEdit;
        public event Func<RolListCLS, Task> OnSearch;
        public event Func<string, Task> OnSearch2;
        public RoleServices(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            lista = new List<RolListCLS>();
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

        public async Task<List<RolListCLS>> listarRole()
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.GetFromJsonAsync<List<RolListCLS>>("api/Rol");
            if (response != null)
            {
                return response;
            }
            return lista;
        }

        public async Task<List<RolListCLS>> buscarRole(string oListRoleCLS)
        {
            
            var lista = await listarRole();
            lista = lista.Where(p =>
                    (string.IsNullOrEmpty(oListRoleCLS) ||
                     p.RoleName.ToUpper().Contains(oListRoleCLS.ToUpper()))


            ).ToList();
            return lista;
        }

        public async Task<RolFormAdd> recuperarRole(int id)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.GetFromJsonAsync<RolFormAdd>("api/Rol/" + id);
                if (response != null)
                {
                    return response;
                }
                return new RolFormAdd();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<bool> agregar(RolFormAdd oPostRoleCLS)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.PostAsJsonAsync("api/Rol", oPostRoleCLS);
            if (response.IsSuccessStatusCode)
            {
                notificarCambios();
                return true;
            }
            return false;
            // listacarrera.Add(oCarreraCLS);

        }

        public async Task<bool> editar(RolFormAdd oPostRoleCLS)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.PutAsJsonAsync("api/Role", oPostRoleCLS);
            if (response.IsSuccessStatusCode)
            {
                notificarCambios();
                return true;
            }
            return false;
            // listacarrera.Add(oCarreraCLS);

        }

        public async Task<bool> eliminar(int idusuario)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.DeleteAsync("api/Rol/" + idusuario);
            if (response.IsSuccessStatusCode)
            {
                notificarCambios();
                return true;
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

        public void notificarSearch(RolListCLS oListRoleCLS)
        {
            OnSearch?.Invoke(oListRoleCLS);
        }

        public void notificarSearch2(string nombrerole)
        {
            OnSearch2?.Invoke(nombrerole);

        }

    }
}
