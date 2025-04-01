using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Models;
using MongoDB.Driver;

namespace iambetter.Application.Services.Database
{
    public class TeamDataService : BaseDataService<Team>
    {
        public TeamDataService(IRepositoryService<Team> repositoryService) : base(repositoryService)
        {
        }

        public async Task AddTeamsAsync(List<ApiTeamResponse> teamResponses, int league)
        {
            await InsertManyAsync(teamResponses.Select(x => new TeamDetails
            {
                Id = x.Team.Id,
                TeamId = x.Team.TeamId,
                Name = x.Team.Name,
                Logo = x.Team.Logo,
                Country = x.Team.Country,
                Founded = x.Team?.Founded,
                Code = x.Team?.Code,
                National = x.Team.National,
                Season = x.Season,
                LeagueId = league
            }));
        }

        public async Task<IEnumerable<int>> GetAllTeamsIdsBySeasonAndLeagueAsync(int league, int season)
        {
            var filter = Builders<Team>.Filter.And(Builders<Team>.Filter.Eq(x => x.LeagueId, league),
                                                    Builders<Team>.Filter.Eq(x => x.Season, season));

            var projection = Builders<Team>.Projection.Include(x => x.TeamId);

            var result = await GetByFilterAsync(filter, projection);

            return result.Select(x => x.TeamId);
        }
    }
}
