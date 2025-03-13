using iambetter.Application.Services.Abstracts;
using iambetter.Application.Services.Interfaces;
using iambetter.Domain.Entities.Models;
using iambetter.Domain.Entities.Projections;

namespace iambetter.Application.Services
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

            await base.InsertAllAsync(list);
        }

    }
}
