using System.Linq.Expressions;
using System.Text.RegularExpressions;
using iambetter.Application.Services.API;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;
using MongoDB.Driver;

namespace iambetter.Application.Services.Database
{
    public class StatsDataService : BaseDataService<MatchDTO>
    {
        public StatsDataService(IRepositoryService<MatchDTO> repositoryService) : base(repositoryService)
        {
        }
        public async Task SaveNextMatchesAsync(IEnumerable<FixtureResponse> fixtureResponses)
        {
            var list = fixtureResponses.Select(fixture => new MatchDTO
            {
                Season = fixture.League.Season,
                Round = fixture.League.Round,
                Teams = fixture.Teams
            }).ToList();

            await InsertManyAsync(list);
        }

        /// <summary>
        /// /// Fills the head to head statistics for the next round matches(like result). The statistics are used to create the dataset
        /// </summary>
        /// <param name="apiService"></param>
        /// <returns></returns>
        public async Task FillHeadToHeadStatistics(APIService apiService)
        {
            var headToHead = await GetNextRoundMatchesAsync(2024, "32");
            var results = await apiService.GetLastHeadToHeadOfAllTeams(headToHead, 2);

            if (results == null || !results.Any())
            {
                throw new Exception("No head to head statistics found for the given season and league ID.");
            }

            //update headToHead 
            foreach(var result in results)
            {
                // find the head to head record to update
                var match = headToHead.FirstOrDefault(m => m.Teams.Home.TeamId == result.Teams.Home.TeamId && m.Teams.Away.TeamId == result.Teams.Away.TeamId && m.Season == result.League.Season && m.Round == GetCleanRound(result.League.Round));
                //evaluate the result and update the result statistic for the match for each Team
                if (match != null)
                {
                    if(result.Teams.Home.Winner.HasValue && result.Teams.Home.Winner.Value)
                        match.Result = "1";
                    else if (result.Teams.Away.Winner.HasValue && result.Teams.Away.Winner.Value)
                        match.Result = "2";
                    else
                        match.Result = "X";
                }
            }
            //upsert with headToHead alaready updated into the collection
            await ReplaceManyAsync(headToHead.Select(m => new ReplaceOneModel<MatchDTO>(Builders<MatchDTO>.Filter.Eq(x => x.Id, m.Id), m)),
                new BulkWriteOptions { IsOrdered = false }
             );
        }

        public async Task<IEnumerable<MatchDTO>> GetNextRoundMatchesAsync(int season, string round)
        {
            //get by filter async
            var filter = Builders<MatchDTO>.Filter.And(
                Builders<MatchDTO>.Filter.Eq(m => m.Season, season),
                Builders<MatchDTO>.Filter.Eq(m => m.Round, round)
            );

            // var projection = Builders<MatchDTO>.Projection.IncludeAll();
            var matches = await GetByFilterAsync(filter);
            return matches;
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
        public async Task AddNextMatchesStatsGroupByRoundAsync(APIService apiDataService, TeamDataService teamDataService, int season, int leagueId)
        {
            try{
                //get all teams for the league and season
                var teamIds = await teamDataService.GetAllTeamsIdsBySeasonAndLeagueAsync(leagueId, season);

                //get all matches for the league and season
                var matches = await apiDataService.GetNextRoundMatches(season, 10);

                await Task.Delay(60000); // Delay to avoid hitting the API rate limit
                
                IEnumerable<TeamStatisticsResponse> statistics = null;
                if(matches != null && matches.Response.Count > 0)
                {
                    //get all statistics for the teams in the matches
                    statistics = await apiDataService.GetAllTeamsStatisticsAsync(teamIds, season);

                    //insert the matches into the database
                    await InsertNextRoundMatchesAsync(matches, statistics);
                }
                else
                {
                    throw new Exception("No matches found for the given season and league ID.");
                }
            }
            catch (Exception ex)
            {
                await Task.Delay(1000);
            }
        }
       

        public async Task InsertNextRoundMatchesAsync(APIRoundResponse matches, IEnumerable<TeamStatisticsResponse> statistics)
        {
            //for each match, get the team statistics for the teams in the match
            foreach (var match in matches.Response)
            {
                var homeTeam = statistics.FirstOrDefault(x => x.Team.TeamId == match.Teams.Home.TeamId);
                var awayTeam = statistics.FirstOrDefault(x => x.Team.TeamId == match.Teams.Away.TeamId);

                if (homeTeam != null && awayTeam != null)
                {
                    var document = new MatchDTO
                    {
                        Season = match.League.Season,
                        Round = GetCleanRound(match.League.Round),
                        Teams = match.Teams,
                        TeamStatistics = new List<TeamStatisticsResponse> { homeTeam, awayTeam }
                    };
                    await base.InsertAsync(document);
                }
            }
        }

        private static string GetCleanRound(string round)
        {
            return Regex.Match(round, @"\d{2}(?!.*\d)").Value;
        }
} 
}
