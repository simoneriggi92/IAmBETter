using iambetter.Application.Services.Abstracts;
using iambetter.Application.Services.Interfaces;
using iambetter.Domain.Entities.Models;

namespace iambetter.Application.Services
{
    public class TeamDataService : BaseDataService<Team>
    {
        public TeamDataService(IRepositoryService<Team> repositoryService) : base(repositoryService)
        {
        }

        public async Task AddTeamsAsync(List<ApiTeamResponse> teamResponses)
        {
            await base.InsertAllAsync(teamResponses.Select(x => new TeamDetails
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
