using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;

namespace iambetter.Application.Services.Database
{
    public class MatchDataService : BaseDataService<MatchDTO>
    {
        public MatchDataService(IRepositoryService<MatchDTO> repositoryService) : base(repositoryService)
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

    }
}
