using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Models;
using System.Text.Json;

namespace iambetter.Application.Services.API
{
    public class APIDataSetService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private const string SERIEA_LEAGUE_ID = "135";

        public APIDataSetService(HttpClient httpClient, IConfiguration configuration)
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

        public async Task<List<ApiTeamResponse>> GetTeamsAsync(int season)
        {
            var url = $"{_baseUrl}teams?league={SERIEA_LEAGUE_ID}&season={season}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve Serie A standings");
            }
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<ApiTeamResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            result?.Response.ForEach(x => x.Season = season);
            return result?.Response;
        }

        public async Task<List<FixtureResponse>> GetNextRoundMatches(int season, int matchesPerRound = 10)
        {
            var url = $"{_baseUrl}fixtures?league={SERIEA_LEAGUE_ID}&season={season}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve upcoming Serie A matches");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<FixtureResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                return new List<FixtureResponse>();

            return result.Response.Where(x => x.Fixture.Status.Status == Domain.Enums.MatchStatus.NS).Take(matchesPerRound).ToList();
        }

        public async Task<TeamStatisticsResponse> GetTeamStatisticsAsync(int teamId, int season)
        {
            var url = $"{_baseUrl}teams/statistics?season={season}&team={teamId}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve team statistics");
            }
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<APIStatsResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result?.Response;
        }
    }
}
