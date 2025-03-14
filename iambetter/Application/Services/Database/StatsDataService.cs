using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Database.Projections;
using MongoDB.Driver;

namespace iambetter.Application.Services.Database
{
    public class StatsDataService : BaseDataService<TeamStatsProjection>
    {
        public StatsDataService(IRepositoryService<TeamStatsProjection> repositoryService) : base(repositoryService)
        {
        }

        public async Task<ReplaceOneResult> UpsertTeamStatsAsync(TeamStatisticsResponse response)
        {
            //check if there is a spcific document for the team
            var filter = Builders<TeamStatsProjection>
                .Filter.And(Builders<TeamStatsProjection>.Filter.Eq(x => x.TeamStatistics.Team.Id, response.Team.Id),
                            Builders<TeamStatsProjection>.Filter.Eq(x => x.TeamStatistics.League.Season, response.League.Season));

            //create document to store
            var document = new TeamStatsProjection
            {
                TeamStatistics = response
            };

            return await base.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = true });
        }

        public async Task<BulkWriteResult<TeamStatsProjection>> UpsertAllTeamsStatsAsync(IEnumerable<TeamStatisticsResponse> responses)
        {
            var bulkOperations = new List<WriteModel<TeamStatsProjection>>();

            foreach (var response in responses)
            {
                var filter = Builders<TeamStatsProjection>
                    .Filter.And(Builders<TeamStatsProjection>.Filter.Eq(x => x.TeamStatistics.Team.Id, response.Team.Id),
                                Builders<TeamStatsProjection>.Filter.Eq(x => x.TeamStatistics.League.Season, response.League.Season));
                var document = new TeamStatsProjection
                {
                    TeamStatistics = response
                };
                var upsertOne = new ReplaceOneModel<TeamStatsProjection>(filter, document)
                {
                    IsUpsert = true
                };
                bulkOperations.Add(upsertOne);
            }

            return await base.ReplaceManyAsync(bulkOperations, new BulkWriteOptions { IsOrdered = false });
        }

        public async Task<IEnumerable<TeamStatsProjection>> GetTeamStatsBySeasonAsync(string season)
        {
            var filter = Builders<TeamStatsProjection>.Filter.Eq(x => x.TeamStatistics.League.Season, Convert.ToInt16(season));
            return await base.GetByFilterAsync(filter);
        }
    }
}
