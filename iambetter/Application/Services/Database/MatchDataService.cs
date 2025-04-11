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
        public MatchDataService(IRepositoryService<MatchDTO> repositoryService) : base(repositoryService)
        {
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
        public async Task GetNextRoundMatchesWithStatsAsync(APIService apiDataService, TeamDataService teamDataService, int leagueId)
        {
            IEnumerable<MatchDTO> matches = null;
            try
            {
                var currentSeason = DateTime.UtcNow.Year - 1; // e.g. 2025 - 1 = 2024
                //get all teams for the league and season
                var teamIds = await teamDataService.GetAllTeamsIdsBySeasonAndLeagueAsync(leagueId, currentSeason);
                
                //Get the maximum number of rounds for the given season and league ID
                var maxRounds = (teamIds.Count() - 1 ) * 2; // e.g. 20 teams = 38 rounds
                var matchesPerRound = teamIds.Count() / 2; // e.g. 20 teams = 10 matches per round
                
                //get the last round from the db
                var lastRound = await GetLastRoundFromDb();
                //compute the number of the potenzial next round match to look for

                if(string.IsNullOrWhiteSpace(lastRound))
                {
                    //get the next round matches from the API
                    var apiResponse = await apiDataService.GetNextRoundMatches(currentSeason, matchesPerRound);
                    matches = GetMatchDTOsFromAPIResponse(apiResponse);
                    await Task.Delay(60000);
                }
                else
                    //check if matches already exist in the db
                    matches = await GetNextRoundMatchesFromDbAsync(apiDataService, currentSeason, lastRound, matchesPerRound);

                if(matches == null)
                    throw new Exception("No matches found for the given season and league ID.");

                //Add the statistics to the matches if they are not already in the db
                if(matches.All(x => x.TeamStatistics == null))
                {
                    //get all statistics for the teams from the API
                    var statistics = await apiDataService.GetAllTeamsStatisticsAsync(teamIds, currentSeason);

                    //insert the matches into the database
                    await InsertNextRoundMatchesAsync(matches, statistics);
                }

                //Check if the last round has been played or not
                if(matches.All(x => x.Status.Short == MatchStatus.FT.ToString()))
                {
                   await SetLastRoundMatchesResults(apiDataService, lastRound);      
                }
            }
            catch (Exception ex)
            {
                await Task.Delay(1000);
            }
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
                Builders<MatchDTO>.Filter.Eq(m => m.Round, round)
            );

            // var projection = Builders<MatchDTO>.Projection.IncludeAll();
            var matches = await GetByFilterAsync(filter, null, null);   

            return matches;
        }


        private static IEnumerable<MatchDTO> GetMatchDTOsFromAPIResponse(APIRoundResponse apiResponse)
        {
            return apiResponse.Response.Select(x => new MatchDTO
            {
                Id = x.Fixture.Id.ToString(),
                Season = x.League.Season,
                Round = GetCleanRound(x.League.Round),
                Teams = x.Teams,
                Result = x.Goals.Home.ToString() + ":" + x.Goals.Away.ToString(),
                Status = x.Status
            }).ToList();
        }

        private async Task<string?> GetLastRoundFromDb()
        {
            //get the last item saved in the db and get the round from it
            var filter = Builders<MatchDTO>.Filter.Empty;
            var sort = Builders<MatchDTO>.Sort.Descending(m => m.Round);
            var projection = Builders<MatchDTO>.Projection.Include(m => m.Round);

            var lastRound = await GetByFilterAsync(Builders<MatchDTO>.Filter.Empty, sort, projection );
            return lastRound.FirstOrDefault()?.Round;
        }

        /// <summary>
        /// Inserts the next round matches into the database. The matches are grouped by round and the statistics are used to create the dataset.
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="statistics"></param>
        /// <returns></returns>
        private async Task InsertNextRoundMatchesAsync(IEnumerable<MatchDTO> matches, IEnumerable<TeamStatisticsResponse> statistics)
        {
            //for each match, get the team statistics for the teams in the match
            foreach (var match in matches)
            {
                var homeTeam = statistics.FirstOrDefault(x => x.Team.TeamId == match.Teams.Home.TeamId);
                var awayTeam = statistics.FirstOrDefault(x => x.Team.TeamId == match.Teams.Away.TeamId);

                if (homeTeam != null && awayTeam != null)
                    match.TeamStatistics = new List<TeamStatisticsResponse> { homeTeam, awayTeam };
               
                    await base.InsertAsync(match);
            }
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
        private async Task SetLastRoundMatchesResults(APIService apiService, string lastRound = null)
        {
            var headToHead = await GetNextRoundMatchesFromDbAsync(apiService, 2024, lastRound);

            //Getting the last head to head statistics to update the results of the matches
            var results = await apiService.GetLastHeadToHeadOfAllTeams(headToHead, 1);

            if (results == null || !results.Any())
            {
                throw new Exception("No head to head statistics found for the given season and league ID.");
            }

            //update headToHead 
            foreach(var result in results)
            {
                // find the head to head record to update (home or away)
                var match = headToHead.FirstOrDefault(m => m.Teams.Home.TeamId == result.Teams.Home.TeamId && m.Teams.Away.TeamId == result.Teams.Away.TeamId
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
            await ReplaceManyAsync(headToHead.Select(m => new ReplaceOneModel<MatchDTO>(Builders<MatchDTO>.Filter.Eq(x => x.Id, m.Id), m)),
                new BulkWriteOptions { IsOrdered = false }
             );
        }

    }
}
