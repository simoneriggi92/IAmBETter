using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Models;

namespace iambetter.Application.Services.Database
{
    public class TeamDataService : BaseDataService<Team>
    {
        public TeamDataService(IRepositoryService<Team> repositoryService) : base(repositoryService)
        {
        }

        public async Task AddTeamsAsync(List<ApiTeamResponse> teamResponses)
        {
            await InsertAllAsync(teamResponses.Select(x => new TeamDetails
            {
                Id = x.Team.Id,
                TeamId = x.Team.TeamId,
                Name = x.Team.Name,
                Logo = x.Team.Logo,
                Country = x.Team.Country,
                Founded = x.Team?.Founded,
                Code = x.Team?.Code,
                National = x.Team.National,
                Season = x.Season
            }));
        }
    }
}
