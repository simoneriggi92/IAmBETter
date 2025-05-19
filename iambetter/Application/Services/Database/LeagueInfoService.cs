using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.DTO;

namespace iambetter.Application.Services.Database
{
    public class LeagueInfoService : BaseDataService<LeagueInfoDTO>
    {
        private readonly ILogger<MatchDataService> _logger;

        public LeagueInfoService(IRepositoryService<LeagueInfoDTO> repositoryService, ILogger<MatchDataService> logger) : base(repositoryService)
        {
            _logger = logger;
        }

        public async Task<LeagueInfoDTO?> GetLeagueInfoAsync()
        {
            return (await GetAllAsync()).SingleOrDefault();
        }
    }
}
