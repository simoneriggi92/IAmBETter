using System.Text.RegularExpressions;
using iambetter.Application.Services.API;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Enums;
using MongoDB.Driver;

namespace iambetter.Application.Services.Database
{
    public class MatchDataService : BaseDataService<MatchDTO>
    {
        private readonly ILogger _logger;

        public MatchDataService(IRepositoryService<MatchDTO> repositoryService, ILogger<MatchDataService> logger) : base(repositoryService)
        {
            _logger = logger;
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
        public async Task SetMatchesResultsAsync(APIService apiDataService, TeamDataService teamDataService, int leagueId)
        {
            int maxRounds, matchesPerRound;
            IEnumerable<MatchDTO> matches;
            var currentSeason = DateTime.UtcNow.Year - 1; // e.g. 2025 - 1 = 2024 -> to be changed with a new logic

            try
            {
                //get all teams for the league and season
                var teamIds = await teamDataService.GetAllTeamsIdsBySeasonAndLeagueAsync(leagueId, currentSeason);

                SetMaxMatchesPerSeasonAndMatchesPerRound(teamIds, out maxRounds, out matchesPerRound);

                var roundToBeChecked = await CalculateRoundToBePlayedAsync(apiDataService, currentSeason, leagueId);

                var matchesToBeChecked = await GetMatchesToBeCkeckedAsync(apiDataService, currentSeason, roundToBeChecked.ToString(), matchesPerRound);

                if (matchesToBeChecked == null)
                {
                    _logger.LogWarning("No matches found for the given season and league ID.");
                    return;
                }

                matchesToBeChecked = await AddStatisticsToEachTeamOfMatchAsync(apiDataService, matchesToBeChecked, currentSeason, teamIds);

                var isResultUpdated = await SetResultsAsync(apiDataService, matchesToBeChecked, currentSeason, leagueId, roundToBeChecked.ToString());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(MatchDataService)}]:: Error while getting the next round matches: {ex.Message}");
                throw;
            }
        }

        private async Task<int> CalculateRoundToBePlayedAsync(APIService apiDataService, int currentSeason, int leagueId)
        {
            //get the last round from the API
            var lastRound = await apiDataService.GetLastRoundFromAPIAsync(leagueId, currentSeason);
            return Convert.ToInt32(GetCleanRound(lastRound));
        }

        private async Task<IEnumerable<MatchDTO>> GetMatchesToBeCkeckedAsync(APIService apiDataService, int currentSeason, string roundToBeChecked, int matchesPerRound)
        {
            var matches = await GetNextRoundMatchesFromDbAsync(apiDataService, currentSeason, roundToBeChecked, matchesPerRound);

            if(matches == null || !matches.Any())
            {
                //get the next round matches from the API
                var apiResponse = await apiDataService.GetNextRoundMatches(currentSeason, matchesPerRound);
                matches = GetMatchDTOsFromAPIResponse(apiResponse);
            }

            return matches.Where(x => x.Round == roundToBeChecked).ToList();
        }

        private async Task<IEnumerable<MatchDTO>> AddStatisticsToEachTeamOfMatchAsync(APIService apiDataService, IEnumerable<MatchDTO> matches, int currentSeason, IEnumerable<int> teamIds)
        {
            //Add the statistics to the matches if they are not already in the db
            if (matches.Any(x => x.TeamStatistics == null) || matches.Any(x => !x.TeamStatistics.Any()))
            {
                var statistics = await SetTeamStatsFromAPIAsync(apiDataService, currentSeason, matches.SelectMany(x => new[] { x.Teams.Home.TeamId, x.Teams.Away.TeamId }).Distinct());

                //insert the matches into the database
                if(statistics == null || !statistics.Any())
                    throw new Exception("No statistics found for the given season and league ID.");

                await InsertNextRoundMatchesAsync(matches, statistics);

                _logger.LogInformation($"[{nameof(MatchDataService)}]:: Matches inserted into the db: {matches.Count()}");
            }

            return matches;
        }


        private async Task<bool> SetResultsAsync(APIService apiDataService, IEnumerable<MatchDTO> matches, int currentSeason, int leagueId, string roundToBeChecked)
        {
            //set the last round matches results if all the matches are finished
            if (matches.Any(x => string.IsNullOrWhiteSpace(x.Result)))
            {
                await SetLastRoundMatchesResults(apiDataService, matches, roundToBeChecked);

                _logger.LogInformation($"[{nameof(MatchDataService)}]:: The last round matches results have been updated in the db");
                return true;
            }
            return false;
        }

        private async Task<IEnumerable<TeamStatisticsResponse?>> SetTeamStatsFromAPIAsync(APIService apiDataService,  int currentSeason, IEnumerable<int> teamIds)
        {
            _logger.LogInformation($"[{nameof(MatchDataService)}]:: Getting the statistics for the teams from the API");
            //get all statistics for the teams from the API
            return await apiDataService.GetAllTeamsStatisticsAsync(teamIds, currentSeason);
        }

        private async Task<(IEnumerable<MatchDTO> matches, string lastRound)> GetNextMatchesToBePlayedfromAPIAsync(APIService apiDataService, int currentSeason, int matchesPerRound)
        {
            _logger.LogWarning($"[{nameof(MatchDataService)}]:: No last round found in the db, getting the next round matches from the API");
            //get the next round matches from the API
            var apiResponse = await apiDataService.GetNextRoundMatches(currentSeason, matchesPerRound);
            var matches = GetMatchDTOsFromAPIResponse(apiResponse);

            var lastRound = GetCleanRound(apiResponse.Response.FirstOrDefault()?.League.Round);
            _logger.LogInformation($"[{nameof(MatchDataService)}]:: Last round: {lastRound}");
            await Task.Delay(60000);
            return (matches, lastRound);
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
        private async Task<IEnumerable<MatchDTO>> GetNextRoundMatchesFromDbAsync(APIService apiService, int season, string round, int matchesPerRound = 10)
        {
            //get by filter async
            var filter = Builders<MatchDTO>.Filter.And(
                Builders<MatchDTO>.Filter.Eq(m => m.Season, season),
                Builders<MatchDTO>.Filter.Eq(m => m.Round, round),
                Builders<MatchDTO>.Filter.Eq(m => m.Result, null)
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
                Result = x.Goals.Home.ToString() + ":" + x.Goals.Away.ToString(),
                Status = x.Status
            }).ToList();
        }

        private async Task<string?> GetLastRoundNumberFromDbAsync(APIService apiService, int maxRounds, int leagueId, int season)
        {
           try
           {
             //get the last item saved in the db and get the round from it
                var filter = Builders<MatchDTO>.Filter.Empty;
                var sort = Builders<MatchDTO>.Sort.Descending(m => m.Round);
                var projection = Builders<MatchDTO>.Projection.Include(m => m.Round);

                var lastMatches = await GetByFilterAsync(Builders<MatchDTO>.Filter.Empty, sort, projection );
                
                int? lastRound = Convert.ToInt32(lastMatches.FirstOrDefault()?.Round);

                //if is there still some matvc to be played or the last round in db is the last one (e.g.38), get the last round from the db, otherwise increase the round by 1
                if( lastRound.HasValue && 
                (lastMatches.Any(x =>x.TeamStatistics.Any(x => string.IsNullOrWhiteSpace(x.Result))) || lastRound == maxRounds))
                    return lastRound.ToString();
                else
                {
                    if(lastRound.HasValue)
                        return lastRound < maxRounds 
                        ? (lastRound + 1).ToString()
                        : null;
                    else
                    {
                        //get round from the API
                        lastRound = Convert.ToInt32(
                            GetCleanRound (
                                await apiService.GetLastRoundFromAPIAsync(leagueId, season)
                                )
                        );
                    }
                }
            return lastRound.ToString();

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


        private async Task<IEnumerable<MatchDTO>> GetAllMatchesAsync()
        {
            var filter = Builders<MatchDTO>.Filter.Empty;
            var matches = await GetByFilterAsync(filter, null, null);
            return matches;
        }

         /// <summary>
        /// /// Fills the head to head statistics for the next round matches(like result). The statistics are used to create the dataset.
        /// The aim of the method is to be invoked after every match weekend in order to update the results of the matches and keep updated the stats to be used for the dataset.
        /// </summary>
        /// <param name="apiService"></param>
        /// <returns></returns>
        private async Task SetLastRoundMatchesResults(APIService apiService, IEnumerable<MatchDTO> matches,  string lastRound = null)
        {
            // var headToHead = await GetNextRoundMatchesFromDbAsync(apiService, 2024, lastRound);

            //Getting the last head to head statistics to update the results of the matches
            var results = await apiService.GetLastHeadToHeadOfAllTeams(matches, 1);

            if (results == null || !results.Any())
            {
                throw new Exception("No head to head statistics found for the given season and league ID.");
            }

            //update headToHead 
            foreach(var result in results)
            {
                // find the head to head record to update (home or away)
                var match = matches.FirstOrDefault(m => m.Teams.Home.TeamId == result.Teams.Home.TeamId && m.Teams.Away.TeamId == result.Teams.Away.TeamId
                 && m.Season == result.League.Season && m.Round == GetCleanRound(result.League.Round));
                //evaluate the result and update the result statistic for the match for each Team
                if (match != null)
                {
                    if(result.Teams.Home.Winner.HasValue && result.Teams.Home.Winner.Value)
                        match.Result = "1";
                    else if (result.Teams.Away.Winner.HasValue && result.Teams.Away.Winner.Value)
                        match.Result = "-1";
                    else
                        match.Result = "0";
                }
            }

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
