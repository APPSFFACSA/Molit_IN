using Molit_IN.Client.Services;
using Molit_IN.Library.Copilots; // CopilotListCLS, CopilotFormAddCLS
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Molit_IN.Client.Services
{
    public class CopilotServices
    {
        public List<CopilotListCLS> lista;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;

        public event Func<Task>? OnChange;
        public event Func<int, Task>? OnEdit;
        public event Func<CopilotListCLS, Task>? OnSearch;
        public event Func<string, Task>? OnSearch2;

        public CopilotServices(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            lista = new List<CopilotListCLS>();
        }

        // Asegura que cada request lleve el Bearer token
        private async Task EnsureAuthorizationAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token) && token != "No token found!")
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Listar todos los copilotos activos
        public async Task<List<CopilotListCLS>> listar()
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.GetFromJsonAsync<List<CopilotListCLS>>("api/Copilot");
            return response ?? lista;
        }

        // Listar por sucursal (BranchCode)
        public async Task<List<CopilotListCLS>> listar(string branchcode)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.GetFromJsonAsync<List<CopilotListCLS>>($"api/Copilot/getCopilotByBranchCode/{branchcode}");
            return response ?? lista;
        }

        // Recuperar un copilot por Id
        // El controlador devuelve CopilotListCLS: mapeamos a CopilotFormAddCLS para usarlo en el formulario.
        public async Task<CopilotFormAddCLS> recuperar(int id)
        {
            try
            {
                await EnsureAuthorizationAsync();
                var view = await _httpClient.GetFromJsonAsync<CopilotListCLS>($"api/Copilot/{id}");
                if (view == null) return new CopilotFormAddCLS();

                return new CopilotFormAddCLS
                {
                    CopilotsId = view.CopilotsId,
                    PilotId = view.PilotId,
                    BranchCode = view.BranchCode,
                    CodEmpleado = view.CodEmpleado,
                    FullName = view.FullName,
                    Age = view.Age,
                    LicenseType = view.LicenseType,
                    LicenseNumber = view.LicenseNumber,
                    LicensePhoto = view.LicensePhoto,
                    NamePhoto = view.NamePhoto,
                    IsActive = view.IsActive,
                    CreatedBy = view.CreatedBy,
                    CreatedDate = view.CreatedDate,
                    UpdatedBy = view.UpdatedBy,
                    UpdatedDate = view.UpdatedDate
                };
            }
            catch (Exception ex)
            {
                // En caso de error, devuelve un DTO vacío (o incluye el mensaje si te sirve para depurar UI)
                return new CopilotFormAddCLS
                {
                    CopilotsId = 0,
                    FullName = $"Error: {ex.Message}"
                };
            }
        }

        // Crear
        public record ApiResult(bool Ok, string? Message);

        public async Task<ApiResult> agregar(CopilotFormAddCLS dto)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.PostAsJsonAsync("api/Copilot", dto);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                notificarCambios();
                return new ApiResult(true, msg);
            }
            return new ApiResult(false, string.IsNullOrWhiteSpace(msg) ? "No se ha podido guardar el registro." : msg);
        }

        // (si usas editar separado)
        public async Task<ApiResult> editar(CopilotFormAddCLS dto)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.PostAsJsonAsync("api/Copilot", dto);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                notificarCambios();
                return new ApiResult(true, msg);
            }
            return new ApiResult(false, string.IsNullOrWhiteSpace(msg) ? "No se ha podido actualizar el registro." : msg);
        }


        // Eliminar lógico
        public async Task<bool> eliminar(int idcopilot)
        {
            await EnsureAuthorizationAsync();
            var response = await _httpClient.DeleteAsync($"api/Copilot/{idcopilot}");
            if (response.IsSuccessStatusCode)
            {
                notificarCambios();
                return true;
            }
            return false;
        }

        // Notificadores
        public void notificarCambios() => OnChange?.Invoke();
        public void notificarEdit(int id) => OnEdit?.Invoke(id);
        public void notificarSearch(CopilotListCLS item) => OnSearch?.Invoke(item);
        public void notificarSearch2(string nombre) => OnSearch2?.Invoke(nombre);
    }
}
