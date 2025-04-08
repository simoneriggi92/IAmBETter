using System.Linq.Expressions;
using System.Text.RegularExpressions;
using iambetter.Application.Services.API;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Database.Projections;
using MongoDB.Driver;

namespace iambetter.Application.Services.Database
{
    public class StatsDataService : BaseDataService<MatchDTO>
    {
        public StatsDataService(IRepositoryService<MatchDTO> repositoryService) : base(repositoryService)
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
        public async Task AddNextMatchesStatsGroupByRoundAsync(APIDataSetService apiDataService, TeamDataService teamDataService, int season, int leagueId)
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
                        Round = Regex.Match(match.League.Round, @"\d{2}(?!.*\d)").Value,
                        Teams = match.Teams,
                        TeamStatistics = new List<TeamStatisticsResponse> { homeTeam, awayTeam }
                    };
                    await base.InsertAsync(document);
                }
            }
        }



        // public async Task<ReplaceOneResult> UpsertTeamStatsAsync(TeamStatisticsResponse response)
        // {
        //     //check if there is a spcific document for the team
        //     var filter = Builders<TeamStatsDTO>
        //         .Filter.And(Builders<TeamStatsDTO>.Filter.Eq(x => x.TeamStatistics.Team.Id, response.Team.Id),
        //                     Builders<TeamStatsDTO>.Filter.Eq(x => x.TeamStatistics.League.Season, response.League.Season));

        //     //create document to store
        //     var document = new TeamStatsDTO
        //     {
        //         TeamStatistics = response
        //     };

        //     return await base.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = true });
        // }


        // public async Task UpsertAllTeamsStatsAsync(IEnumerable<TeamStatisticsResponse> responses)
        // {
        //     var documents = new List<TeamStatsDTO>();

        //     foreach (var response in responses)
        //     {
        //         var filter = Builders<TeamStatsDTO>
        //             .Filter.And(Builders<TeamStatsDTO>.Filter.Eq(x => x.TeamStatistics.Team.TeamId, response.Team.TeamId),
        //                         Builders<TeamStatsDTO>.Filter.Eq(x => x.TeamStatistics.League.Season, response.League.Season));
        //         var document = new TeamStatsDTO
        //         {
        //             TeamStatistics = response
        //         };
        //         documents.Add(document);
        //     }

        //     await base.InsertManyAsync(documents);
        // }

        // public async Task<IEnumerable<TeamStatsDTO>> GetTeamStatsBySeasonAsync(string season)
        // {
        //     var filter = Builders<TeamStatsDTO>.Filter.Eq(x => x.TeamStatistics.League.Season, Convert.ToInt16(season));
        //     return await base.GetByFilterAsync(filter);
        // }
    }
}
