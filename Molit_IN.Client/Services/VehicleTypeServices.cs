
using Molit_IN.Client.Services;
using Molit_IN.Library.VehicleType;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Molit_IN.Client.Services
{
    public class VehicleTypeServices
    {
        public List<VehicleListCLS> lista;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;
        public event Func<Task> OnChange;
        public event Func<int, Task> OnEdit;
        public event Func<VehicleListCLS, Task> OnSearch;
        public VehicleTypeServices(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            lista = new List<VehicleListCLS>();
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

        public async Task<List<VehicleListCLS>> listar()
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.GetFromJsonAsync<List<VehicleListCLS>>("api/VehicleType");
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

        public async Task<VehicleFormAddCLS> recuperar(int id)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.GetFromJsonAsync<VehicleFormAddCLS>("api/VehicleType/" + id);
                if (response != null)
                {
                    return response;
                }
                return new VehicleFormAddCLS();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<bool> agregar(VehicleFormAddCLS oVehicleFormAddCLS)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.PostAsJsonAsync("api/VehicleType", oVehicleFormAddCLS);
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

        public async Task<bool> editar(VehicleFormAddCLS oVehicleFormAddCLS)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.PutAsJsonAsync("api/VehicleType", oVehicleFormAddCLS);
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

        public async Task<bool> eliminar(int id)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var response = await _httpClient.DeleteAsync("api/VehicleType/" + id);
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

        public void notificarEdit(int id)
        {
            OnEdit?.Invoke(id);
        }

        public void notificarSearch(VehicleListCLS oVehicleListCLS)
        {
           OnSearch?.Invoke(oVehicleListCLS);
        }
    }
}
