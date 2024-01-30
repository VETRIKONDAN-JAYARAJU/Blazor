using DataObjects;
using Dapper;
using System.Net.Http;
using System.Net.Http.Json;

namespace DataServices
{
    public class SettlementService
    {
        private readonly HttpClient httpClient;

        List<Settlement>? listSettlement = new();

        public SettlementService(HttpClient _httpClient)
        {
            this.httpClient = _httpClient;
        }

        public async Task<List<Settlement>> SelectAll()
        {
            listSettlement = await httpClient.GetFromJsonAsync<List<Settlement>>("https://localhost:7245/api/Settlement/Get");
            return listSettlement;
        }
    }
}
