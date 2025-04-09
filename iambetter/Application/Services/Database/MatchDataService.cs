using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;
using MongoDB.Driver;

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

        public async Task<IEnumerable<MatchDTO>> GetNextRoundMatchesAsync(int season, string round)
        {
            //get by filter async
            var filter = Builders<MatchDTO>.Filter.And(
                Builders<MatchDTO>.Filter.Eq(m => m.Season, season),
                Builders<MatchDTO>.Filter.Eq(m => m.Round, round)
            );

            var projection = Builders<MatchDTO>.Projection.Include(m => m.Teams);
            var matches = await GetByFilterAsync(filter, projection);
            return matches.Select(m => new MatchDTO
            {
                Season = m.Season,
                Round = m.Round,
                Teams = m.Teams
            }).ToList();
        }
    }
}
