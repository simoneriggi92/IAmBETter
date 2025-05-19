using iambetter.Application.Services.API;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Application.Services.Interfaces;
using iambetter.Domain.Entities.AI.Response;
using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Database.DTO;
using iambetter.Domain.Entities.Database.Projections;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace iambetter.Application.Services.Database
{
    public class MatchDataService : BaseDataService<MatchDTO>
    {
        private readonly ILogger _logger;
        private readonly APIService _apiDataService;
        private readonly LeagueInfoService _leagueInfoService;

        public MatchDataService(IRepositoryService<MatchDTO> repositoryService, ILogger<MatchDataService> logger, APIService apiService, BaseDataService<LeagueInfoDTO> leagueInfoService) : base(repositoryService)
        {
            _logger = logger;
            _apiDataService = apiService;
            _leagueInfoService = leagueInfoService as LeagueInfoService;
        }

        /// <summary>
        /// Adds the next matches statistics grouped by round for the given season and league ID. After the round the statistics are used to create the dataset
        /// to train the AI model. The statistics are grouped by round and inserted into the database.
        /// </summary>
        /// <param name="apiDataService"></param>
        /// <param name="teamDataService"></param>
        /// <param name="season"></param>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public async Task SetMatchesResultsAsync(FastAPIDataService fastAPIDataService, TeamDataService teamDataService, PredictionService predictionService, IAIDataSetService dataSetComposerService, int leagueId, bool skipRoundComputationFromAPI = false)
        {
            int maxRounds, matchesPerRound;
            var leagueInfo = await _leagueInfoService.GetLeagueInfoAsync();
            if (leagueInfo == null)
            {
                _logger.LogError($"[{nameof(MatchDataService)}]:: no league info stored in the db");
                return;
            }

            var currentSeason = DateTime.UtcNow.Year - 1; // e.g. 2025 - 1 = 2024 -> to be changed with a new logic

            try
            {
                //get all teams for the league and season
                var teamIds = await teamDataService.GetAllTeamsIdsBySeasonAndLeagueAsync(leagueId, currentSeason);

                SetMaxMatchesPerSeasonAndMatchesPerRound(teamIds, out maxRounds, out matchesPerRound);

                var roundToBeChecked = 0;

                if (skipRoundComputationFromAPI)
                    //get the last round from the db
                    roundToBeChecked = Convert.ToInt32(await GetLastRoundNumberFromDbAsync(_apiDataService, maxRounds, leagueId, currentSeason));
                else
                    roundToBeChecked = await CalculateRoundToBePlayedAsync(currentSeason, leagueId);

                //no league info stored in the db
                leagueInfo.CurrentRound = roundToBeChecked.ToString();

                var filter = Builders<LeagueInfoDTO>.Filter.Eq(x => x.Id, leagueInfo.Id);
                await _leagueInfoService.ReplaceOneAsync(filter, leagueInfo, new ReplaceOptions { IsUpsert = true });

                var matchesToBeChecked = await GetMatchesToBeCkeckedAsync(currentSeason, roundToBeChecked.ToString(), matchesPerRound);

                if (matchesToBeChecked == null)
                {
                    _logger.LogWarning("No matches found for the given season and league ID.");
                    return;
                }

                matchesToBeChecked = await AddStatisticsToEachTeamOfMatchAsync(matchesToBeChecked, currentSeason, teamIds);

                var isResultUpdated = await SetResultsAsync(matchesToBeChecked, currentSeason, leagueId, roundToBeChecked.ToString());

                //update the dataset with the new results
                if (isResultUpdated)
                {
                    matchesToBeChecked = await GetAllMatchesWithResultAsync();
                    dataSetComposerService.GenerateDataSet(matchesToBeChecked);
                    _logger.LogInformation($"[{nameof(MatchDataService)}]:: The dataset has been updated with the new results");
                    await fastAPIDataService.PostCsvAsync(dataSetComposerService.GetCsvFilePath(), "/train");
                }


                //Get the matches without prediction saved in the db
                var matchesToBePredicted = GetMatchesToBePredicted(roundToBeChecked, matchesToBeChecked);

                if (matchesToBePredicted != null && matchesToBePredicted.Any())
                {
                    var predictions = await GetPredictionsFromAPIAsync(fastAPIDataService, dataSetComposerService, matchesToBeChecked);

                    if (predictions != null)
                    {
                        IEnumerable<PredictionDTO> predictionDTOs = GetPredictionDTOs(matchesToBeChecked, predictions);

                        await predictionService.SavePredictionsAsync(predictionDTOs);

                        //Set to true the predicted flag for the matches to be predicted and upsert with headToHead alaready updated into the collection
                        matchesToBePredicted.ForEach(x => x.Predicted = true);

                        await UpdateMatchesAsync(matchesToBePredicted);

                        _logger.LogInformation($"[{nameof(MatchDataService)}]:: The predictions have been saved into the db");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(MatchDataService)}]:: Error while getting the next round matches: {ex.Message}");
                throw;
            }
        }

        private static async Task<PredictionResponse?> GetPredictionsFromAPIAsync(FastAPIDataService fastAPIDataService, IAIDataSetService dataSetComposerService, IEnumerable<MatchDTO> matchesToBeChecked)
        {
            dataSetComposerService.GenerateDataSet(matchesToBeChecked);
            var result = await fastAPIDataService.PostCsvAsync(dataSetComposerService.GetCsvFilePath(), "/predict");
            var predictions = JsonConvert.DeserializeObject<PredictionResponse>(result);
            return predictions;
        }

        private static List<MatchDTO> GetMatchesToBePredicted(int roundToBeChecked, IEnumerable<MatchDTO> matchesToBeChecked)
        {
            return matchesToBeChecked.Where(x => !x.Predicted && x.Round == roundToBeChecked.ToString()).ToList();
        }

        private static IEnumerable<PredictionDTO> GetPredictionDTOs(IEnumerable<MatchDTO> matchesToBeChecked, PredictionResponse predictions)
        {
            return predictions.Predictions.Select(x => new PredictionDTO
            {
                HomeTeam = new Domain.Entities.Models.Team
                {
                    TeamId = x.HomeTeamId,
                    Name = matchesToBeChecked.FirstOrDefault(m => m.Teams.Home.TeamId == x.HomeTeamId && m.Teams.Away.TeamId == x.AwayTeamId).Teams.Home.Name,
                    Logo = matchesToBeChecked.FirstOrDefault(m => m.Teams.Home.TeamId == x.HomeTeamId && m.Teams.Away.TeamId == x.AwayTeamId).Teams.Home.Logo
                },
                AwayTeam = new Domain.Entities.Models.Team
                {
                    TeamId = x.AwayTeamId,
                    Name = matchesToBeChecked.FirstOrDefault(m => m.Teams.Home.TeamId == x.HomeTeamId && m.Teams.Away.TeamId == x.AwayTeamId).Teams.Away.Name,
                    Logo = matchesToBeChecked.FirstOrDefault(m => m.Teams.Home.TeamId == x.HomeTeamId && m.Teams.Away.TeamId == x.AwayTeamId).Teams.Away.Logo
                },
                MatchDate = matchesToBeChecked.FirstOrDefault(m => m.Teams.Home.TeamId == x.HomeTeamId && m.Teams.Away.TeamId == x.AwayTeamId).MatchDate,
                Round = matchesToBeChecked.FirstOrDefault(m => m.Teams.Home.TeamId == x.HomeTeamId && m.Teams.Away.TeamId == x.AwayTeamId).Round,
                PredictedResult = x.PredictedResult
            });
        }

        public async Task<int> CalculateRoundToBePlayedAsync(int currentSeason, int leagueId, bool applyDelay = true)
        {
            //Check if is there still a round already in the db with results to be updated (the previous one)
            var lastRound = await GetLastRoundNumberFromDbAsync(_apiDataService, 38, leagueId, currentSeason);

            if (string.IsNullOrWhiteSpace(lastRound))
            {

                //get the last round from the API
                lastRound = await _apiDataService.GetLastRoundFromAPIAsync(leagueId, currentSeason);

                if (applyDelay)
                    await Task.Delay(70000); // wait for one minute before to perform the next API call 

            }

            return Convert.ToInt32(GetCleanRound(lastRound));
        }

        private async Task<IEnumerable<MatchDTO>> GetMatchesToBeCkeckedAsync(int currentSeason, string roundToBeChecked, int matchesPerRound)
        {
            var matches = await GetNextRoundMatchesFromDbAsync(currentSeason, roundToBeChecked, matchesPerRound);

            if (matches == null || !matches.Any())
            {
                //get the next round matches from the API
                var apiResponse = await _apiDataService.GetNextRoundMatches(currentSeason, matchesPerRound);
                matches = GetMatchDTOsFromAPIResponse(apiResponse);
                await Task.Delay(70000);
            }

            return matches.Where(x => x.Round == roundToBeChecked).ToList();
        }

        private async Task<IEnumerable<MatchDTO>> AddStatisticsToEachTeamOfMatchAsync(IEnumerable<MatchDTO> matches, int currentSeason, IEnumerable<int> teamIds)
        {
            //Add the statistics to the matches if they are not already in the db
            if (matches.Any(x => x.TeamStatistics == null) || matches.Any(x => !x.TeamStatistics.Any()))
            {
                var statistics = await SetTeamStatsFromAPIAsync(currentSeason, matches.SelectMany(x => new[] { x.Teams.Home.TeamId, x.Teams.Away.TeamId }).Distinct());

                //insert the matches into the database
                if (statistics == null || !statistics.Any())
                    throw new Exception("No statistics found for the given season and league ID.");

                await InsertNextRoundMatchesAsync(matches, statistics);

                _logger.LogInformation($"[{nameof(MatchDataService)}]:: Matches inserted into the db: {matches.Count()}");
            }

            return matches;
        }


        private async Task<bool> SetResultsAsync(IEnumerable<MatchDTO> matches, int currentSeason, int leagueId, string roundToBeChecked)
        {
            var result = false;
            //set the last round matches results if all the matches are finished
            if (matches.Any(x => string.IsNullOrWhiteSpace(x.Result)))
            {
                result = await SetLastRoundMatchesResults(matches, roundToBeChecked);

                if (result)
                    _logger.LogInformation($"[{nameof(MatchDataService)}]:: The last round matches results have been updated in the db");
            }
            return result;
        }

        private async Task<IEnumerable<TeamStatisticsResponse?>> SetTeamStatsFromAPIAsync(int currentSeason, IEnumerable<int> teamIds)
        {
            _logger.LogInformation($"[{nameof(MatchDataService)}]:: Getting the statistics for the teams from the API");
            //get all statistics for the teams from the API
            return await _apiDataService.GetAllTeamsStatisticsAsync(teamIds, currentSeason);
        }

        private static void SetMaxMatchesPerSeasonAndMatchesPerRound(IEnumerable<int> teamIds, out int maxRounds, out int matchesPerRound)
        {
            //Get the maximum number of rounds for the given season and league ID
            maxRounds = (teamIds.Count() - 1) * 2;
            matchesPerRound = teamIds.Count() / 2;
            // e.g. 20 teams = 10 matches per round
        }

        /// <summary>
        /// Get the next 10 round matches to be stored in the db.
        /// </summary>
        /// <param name="season"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        private async Task<IEnumerable<MatchDTO>> GetNextRoundMatchesFromDbAsync(int season, string round, int matchesPerRound = 10)
        {
            //get by filter async
            var filter = Builders<MatchDTO>.Filter.And(
                Builders<MatchDTO>.Filter.Eq(m => m.Season, season),
                Builders<MatchDTO>.Filter.Eq(m => m.Round, round),
                Builders<MatchDTO>.Filter.Eq(m => m.Result, string.Empty)
            );

            // var projection = Builders<MatchDTO>.Projection.IncludeAll();
            var matches = await GetByFilterAsync(filter, null, null);

            return matches;
        }


        private static IEnumerable<MatchDTO> GetMatchDTOsFromAPIResponse(APIRoundResponse apiResponse)
        {
            return apiResponse.Response.Select(x => new MatchDTO
            {
                Season = x.League.Season,
                Round = GetCleanRound(x.League.Round),
                Teams = x.Teams,
                FinalScore = x.Goals.Home.ToString() + ":" + x.Goals.Away.ToString(),
                Status = x.Status,
                MatchDate = Convert.ToDateTime(x.Fixture.Date)

            }).ToList();
        }

        private async Task<string?> GetLastRoundNumberFromDbAsync(APIService apiService, int maxRounds, int leagueId, int season)
        {
            try
            {
                //get the last item saved in the db and get the round from it
                var filter = Builders<MatchDTO>.Filter.And(
                    Builders<MatchDTO>.Filter.Eq(m => m.Result, string.Empty)
                );

                var sort = Builders<MatchDTO>.Sort.Descending(m => m.Round);
                var projection = Builders<MatchDTO>.Projection.Include(m => m.Round).Include(m => m.Result);

                var lastMatches = await GetByFilterAsync(Builders<MatchDTO>.Filter.Empty, sort, projection);

                int? lastRound = Convert.ToInt32(lastMatches.FirstOrDefault(x => x.Result == string.Empty)?.Round);

                if (lastRound == 0)
                    return null;

                //if is there still some matvc to be played or the last round in db is the last one (e.g.38), get the last round from the db, otherwise increase the round by 1
                if (lastRound.HasValue)
                    return lastRound.ToString();

                return null;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(MatchDataService)}]:: Error while getting the last round:: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Inserts the next round matches into the database. The matches are grouped by round and the statistics are used to create the dataset.
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="statistics"></param>
        /// <returns></returns>
        private async Task InsertNextRoundMatchesAsync(IEnumerable<MatchDTO> matches, IEnumerable<TeamStatisticsResponse> statistics)
        {
            var replaceOptions = new ReplaceOptions { IsUpsert = true };

            //for each match, get the team statistics for the teams in the match
            foreach (var match in matches)
            {
                var homeTeam = statistics.FirstOrDefault(x => x.Team.TeamId == match.Teams.Home.TeamId);
                var awayTeam = statistics.FirstOrDefault(x => x.Team.TeamId == match.Teams.Away.TeamId);

                if (homeTeam != null && awayTeam != null)
                    match.TeamStatistics = new List<TeamStatisticsResponse> { homeTeam, awayTeam };
            }
            //   var filter = Builders<MatchDTO>.Filter.And(
            //     Builders<MatchDTO>.Filter.Eq(m => m.Season, match.Season),
            //     Builders<MatchDTO>.Filter.Eq(m => m.Round, match.Round),
            //     Builders<MatchDTO>.Filter.Eq(m => m.Teams.Home.TeamId, match.Teams.Home.TeamId));

            await base.InsertManyAsync(matches);
        }

        /// <summary>
        /// Cleans the round string to get the round number. The round string is in the format "Round - 1" or "Round - 2" and we want to get only the number.
        /// </summary>
        /// <param name="round"></param>
        /// <returns></returns>
        private static string GetCleanRound(string round)
        {
            return Regex.Match(round, @"\d{2}(?!.*\d)").Value;
        }


        private async Task<IEnumerable<MatchDTO>> GetAllMatchesWithResultAsync()
        {
            var filter = Builders<MatchDTO>.Filter.And(
                Builders<MatchDTO>.Filter.Ne(m => m.Result, string.Empty)
            );

            var matches = await GetByFilterAsync(filter, sortDefinition: null, null);
            return matches;
        }

        private async Task<IEnumerable<MatchDTO>> GetAllMatchesWithNoResultAsync()
        {
            var filter = Builders<MatchDTO>.Filter.And(
                Builders<MatchDTO>.Filter.Eq(m => m.Result, string.Empty)
            );

            var matches = await GetByFilterAsync(filter, sortDefinition: null, null);
            return matches;
        }

        /// <summary>
        /// /// Fills the head to head statistics for the next round matches(like result). The statistics are used to create the dataset.
        /// The aim of the method is to be invoked after every match weekend in order to update the results of the matches and keep updated the stats to be used for the dataset.
        /// </summary>
        /// <param name="apiService"></param>
        /// <returns></returns>
        private async Task<bool> SetLastRoundMatchesResults(IEnumerable<MatchDTO> matches, string lastRound = null)
        {
            var setResults = false;
            // var headToHead = await GetNextRoundMatchesFromDbAsync(apiService, 2024, lastRound);

            //Getting the last head to head statistics to update the results of the matches
            var results = await _apiDataService.GetLastHeadToHeadOfAllTeams(matches, 1);

            if (results == null || !results.Any())
            {
                _logger.LogWarning("No head to head statistics found for the given season and league ID.");
                return false;
            }

            //update headToHead 
            foreach (var result in results)
            {
                // find the head to head record to update (home or away)
                var match = matches.FirstOrDefault(m => m.Teams.Home.TeamId == result.Teams.Home.TeamId && m.Teams.Away.TeamId == result.Teams.Away.TeamId
                 && m.Season == result.League.Season && m.Round == GetCleanRound(result.League.Round));
                //evaluate the result and update the result statistic for the match for each Team
                if (match != null)
                {
                    if (result.Teams.Home.Winner.HasValue && result.Teams.Home.Winner.Value)
                        match.Result = "1";
                    else if (result.Teams.Away.Winner.HasValue && result.Teams.Away.Winner.Value)
                        match.Result = "-1";
                    else
                        match.Result = "0";

                    setResults = true;
                }
            }

            if (setResults)
                await UpdateMatchesAsync(matches);

            return setResults;
        }

        private async Task UpdateMatchesAsync(IEnumerable<MatchDTO> matches)
        {
            //upsert with headToHead alaready updated into the collection
            await ReplaceManyAsync(matches.Select(m => new ReplaceOneModel<MatchDTO>(Builders<MatchDTO>.Filter.Eq(x => x.Id, m.Id), m)),
                new BulkWriteOptions { IsOrdered = false }
             );
        }

        public bool IsThereAnyMatchToBePlayed(string round, int season)
        {
            var filter = Builders<MatchDTO>.Filter.And(
                Builders<MatchDTO>.Filter.Eq(m => m.Season, season),
                Builders<MatchDTO>.Filter.Eq(m => m.Round, round)
            );

            var matches = GetByFilterAsync(filter, null, null).Result;
            return matches.Any(x => string.IsNullOrWhiteSpace(x.Status.Short));
        }
    }
}
