using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;
using System.Text.Json;

namespace iambetter.Application.Services.API
{
    public class APIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private const string SERIEA_LEAGUE_ID = "135";
        private const int DELAY_BETWEEN_REQUESTS = 60000;
        private const int MAX_REQUESTS_PER_MINUTE = 10;

        public APIService(HttpClient httpClient, IConfiguration configuration)
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


        public async Task<IEnumerable<TeamStatisticsResponse>> GetAllTeamsStatisticsAsync(IEnumerable<int> teamdIds, int season)
        {
            //we can do only 10 requests/minute, so we need to split the ids to tasks batches of 10 and wait for one minute between each batch
            var results = new List<TeamStatisticsResponse>();

            //create list of list of 10 tasks 
            var batches = teamdIds.
                Select((teamId, index) => new { teamId, index })
                .GroupBy(x => x.index / MAX_REQUESTS_PER_MINUTE)
                .Select(g => g.Select(x => x.teamId).ToList())
                .ToList();

            foreach (var batch in batches)
            {
                var batchResult = await Task.WhenAll(batch.Select(x => GetTeamStatisticsAsync(x, season)));

                //check if any of the tasks failed
                if (batchResult.Any(x => x == null))
                {
                    throw new Exception("Failed to retrieve team statistics for one or more teams");
                }
                                
                results.AddRange(batchResult);

                //wait before the next batch, except for the last one
                if (batch != batches.Last())
                    await Task.Delay(DELAY_BETWEEN_REQUESTS);

            }
            return results;
        }

        public async Task<TeamStatisticsResponse?> GetTeamStatisticsAsync(int teamId, int season)
        {
            var url = $"{_baseUrl}teams/statistics?season={season}&team={teamId}&league={SERIEA_LEAGUE_ID}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve team statistics");
            }
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<APIStatsResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result?.Response;
        }

        public async Task<APIRoundResponse> GetNextRoundMatches(int season, int matchesPerRound = 10)
        {
            var url = $"{_baseUrl}fixtures?league={SERIEA_LEAGUE_ID}&season={season}&next={matchesPerRound}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve upcoming Serie A matches");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<APIRoundResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                return new APIRoundResponse();

            return result;
        }


        public async Task<IEnumerable<FixtureResponse>?> GetLastHeadToHeadOfAllTeams(IEnumerable<MatchDTO> headToHead, int last = 1)
        {
           //Create a list of team ids from the headToHead list
            var teamdIds = headToHead.Select(x => new { team1Id = Convert.ToInt32(x.Teams.Home.TeamId), team2Id = Convert.ToInt32(x.Teams.Away.TeamId) }).ToList();

            //we can do only 10 requests/minute, so we need to split the ids to tasks batches of 10 and wait for one minute between each batch     

           var results = new List<FixtureResponse>();
           //create list of list of 10 tasks 
            var batches = teamdIds.
                Select((teamId, index) => new { teamId, index })
                .GroupBy(x => x.index / MAX_REQUESTS_PER_MINUTE)
                .Select(g => g.Select(x => x.teamId).ToList())
                .ToList();

            foreach (var batch in batches)
            {
                var batchResult = await Task.WhenAll(batch.Select(x => GetLastHeadToHead(x.team1Id, x.team2Id, last)));

                //check if any of the tasks failed
                if (batchResult.Any(x => x == null))
                {
                    throw new Exception("Failed to retrieve team statistics for one or more teams");
                }

                results.AddRange(batchResult.SelectMany(x => x.Response));

                //wait before the next batch, except for the last one
                if (batch != batches.Last())
                    await Task.Delay(DELAY_BETWEEN_REQUESTS);
            }
            return results;
        }

        private async Task<APIRoundResponse>  GetLastHeadToHead(int team1Id, int team2Id, int last)
        {
            var url = $"{_baseUrl}fixtures/headtohead?h2h={team1Id}-{team2Id}&last={last}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve upcoming Serie A matches");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<APIRoundResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                return new APIRoundResponse();

            return result;
        }
    }
}
