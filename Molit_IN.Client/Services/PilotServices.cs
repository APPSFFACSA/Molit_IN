using Molit_IN.Client.Services;
using Molit_IN.Library.Pilot;
using Molit_IN.Library.Rol;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Molit_IN.Client.Services
{
    public class PilotServices
    {
        public List<PilotListCLS> lista;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;
        public event Func<Task> OnChange;
        public event Func<int, Task> OnEdit;
        public event Func<PilotListCLS, Task> OnSearch;
        public event Func<string, Task> OnSearch2;
        public PilotServices(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            lista = new List<PilotListCLS>();
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

        public async Task<List<PilotListCLS>> listar(string branchcode)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.GetFromJsonAsync<List<PilotListCLS>>("api/Pilot/getPilotByBranchCode/" + branchcode); 
            if (response != null)
            {
                return response;
            }
            return lista;
        }


        public async Task<PilotFormAddCLS> recuperar(int id)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.GetFromJsonAsync<PilotFormAddCLS>("api/Pilot/" + id);
                if (response != null)
                {
                    return response;
                }
                return new PilotFormAddCLS();
            }
            catch (Exception ex)
            {
                PilotFormAddCLS oPilotFormAddCLS = new PilotFormAddCLS();
                oPilotFormAddCLS.PilotId = 0;
                oPilotFormAddCLS.FullName = $"Error: {ex.Message}";
                return new PilotFormAddCLS();
            }

        }

        public async Task<bool> agregar(PilotFormAddCLS oPilotFormAddCLS)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.PostAsJsonAsync("api/Pilot", oPilotFormAddCLS);
            if (response.IsSuccessStatusCode)
            {
                notificarCambios();
                return true;
            }
            return false;
            // listacarrera.Add(oCarreraCLS);

        }

        public async Task<bool> editar(PilotFormAddCLS oPilotFormAddCLS)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.PutAsJsonAsync("api/Pilot", oPilotFormAddCLS);
            if (response.IsSuccessStatusCode)
            {
                notificarCambios();
                return true;
            }
            return false;
            // listacarrera.Add(oCarreraCLS);

        }

        public async Task<bool> eliminar(int idpilot)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.DeleteAsync($"api/Pilot/{idpilot}");
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

        public void notificarSearch(PilotListCLS oPilotListCLS)
        {
            OnSearch?.Invoke(oPilotListCLS);
        }

        public void notificarSearch2(string nombre)
        {
            OnSearch2?.Invoke(nombre);

        }
    }
}
