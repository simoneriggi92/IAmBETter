using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;

namespace iambetter.Application.Services.Database
{
    public class MatchDataService : BaseDataService<MatchProjection>
    {
        public MatchDataService(IRepositoryService<MatchProjection> repositoryService) : base(repositoryService)
        {
        }

        public async Task SveNextMatchesAsync(IEnumerable<FixtureResponse> fixtureResponses)
        {
            var list = fixtureResponses.Select(fixture => new MatchProjection
            {
                Season = fixture.League.Season,
                Round = fixture.League.Round,
                Fixture = fixture.Fixture,
                Teams = fixture.Teams
            }).ToList();

            await InsertAllAsync(list);
        }

    }
}
