using iambetter.Application.Services.Abstracts;
using iambetter.Application.Services.Interfaces;
using iambetter.Domain.Entities.Projections;

namespace iambetter.Application.Services
{
    public class StatsDataService : BaseDataService<TeamStasProjection>
    {
        public StatsDataService(IRepositoryService<TeamStasProjection> repositoryService) : base(repositoryService)
        {
        }

    }
}
