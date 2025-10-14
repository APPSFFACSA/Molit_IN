// Client/Services/BranchServices.cs
using System.Net.Http.Json;
using Molit_IN.Library.Branch;

namespace Molit_IN.Client.Services
{
    public sealed class BranchServices
    {
        private readonly HttpClient _http;

        public BranchServices(HttpClient http)
        {
            _http = http;
        }

        public async Task<IReadOnlyList<TiendasCLS>> GetTiendasAsync(CancellationToken ct = default)
        {
            var data = await _http.GetFromJsonAsync<List<TiendasCLS>>("api/Store/tiendas", ct);
            return data ?? new List<TiendasCLS>();
        }

        public Task<TiendasCLS?> GetTiendaByCodeAsync(string code, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(code)) return Task.FromResult<TiendasCLS?>(null);
            return _http.GetFromJsonAsync<TiendasCLS>($"api/Store/tiendas/{Uri.EscapeDataString(code)}", ct);
        }
    }
}
