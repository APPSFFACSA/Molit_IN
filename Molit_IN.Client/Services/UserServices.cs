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
        public event Func<Task> OnChange;
        public event Func<int, Task> OnEdit;
        public event Func<UserListCLS, Task> OnSearch;
        public event Func<string, Task> OnSerach2;

        public UserServices(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            lista = new List<UserListCLS>();
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


        public async Task<List<UserListCLS>> listarUsuario()
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.GetFromJsonAsync<List<UserListCLS>>("api/User");
            if (response != null)
            {
                return response;
            }
            return lista;
        }

        public async Task<List<UserListCLS>> buscarUsuario(string oUsuarioListCLS)
        {
            var lista = await listarUsuario();
            lista = lista.Where(p =>
                    (string.IsNullOrEmpty(oUsuarioListCLS) ||
                     p.UserName.ToUpper().Contains(oUsuarioListCLS.ToUpper()))
            ).ToList();
            return lista;
        }

        public async Task<UserFormAddCLS> recuperarUsuario(int idusuario)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.GetFromJsonAsync<UserFormAddCLS>("api/User/" + idusuario);
            if (response != null)
            {
                return response;
            }
            return new UserFormAddCLS();
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


        public void notificarCambios()
        {
            OnChange?.Invoke();
        }

        public void notificarEdit(int idcarrera)
        {
            OnEdit?.Invoke(idcarrera);
        }

        public void notificarSearch(UserListCLS oUsuarioListCLS)
        {
            OnSearch?.Invoke(oUsuarioListCLS);
        }

        public void notificarSearch2(string nombreusuario)
        {
            OnSerach2?.Invoke(nombreusuario);

        }

    }
}
