using iambetter.Domain.Models;
using System.Text.Json;

namespace iambetter.Application.Services
{
    public class DataSetService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private const string SERIEA_LEAGUE_ID = "135";

        public DataSetService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _baseUrl = _configuration["ApiFootball:BaseUrl"];
            _httpClient.DefaultRequestHeaders.Add("x-apisports-key", _configuration["ApiFootball:ApiKey"]);
        }

        /// <summary>
        /// Get upcoming Serie A matches for the current season
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<List<FixtureResponse>?> GetSerieAMatchesAsync(int season)
        {
            var url = $"{_baseUrl}fixtures?league={SERIEA_LEAGUE_ID}&season={season}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve upcoming Serie A matches");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<FixtureResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result?.Response;
        }

        //public async Task<List<object>> GetTeamsStatsAsync(int season)
        //{
        //    var url = $"{_baseUrl}teams/statistics?league={SERIEA_LEAGUE_ID}&season={season}";
        //    var response = await _httpClient.GetAsync(url);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new Exception("Failed to retrieve Serie A standings");
        //    }
        //    var json = await response.Content.ReadAsStringAsync();
        //    var result = JsonSerializer.Deserialize<ApiResponse<FixtureResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //    return result?.Response;
        //}

        public async Task<List<TeamResponse>> GetTeamsAsync(int season)
        {
            var url = $"{_baseUrl}teams?league={SERIEA_LEAGUE_ID}&season={season}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve Serie A standings");
            }
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<TeamResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result?.Response;
        }
    }
}
