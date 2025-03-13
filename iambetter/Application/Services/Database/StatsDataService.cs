using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Database.Projections;
using MongoDB.Driver;

namespace iambetter.Application.Services.Database
{
    public class StatsDataService : BaseDataService<TeamStasProjection>
    {
        public StatsDataService(IRepositoryService<TeamStasProjection> repositoryService) : base(repositoryService)
        {
        }

        public async Task<ReplaceOneResult> UpsertTeamStatsAsync(TeamStatisticsResponse response)
        {
            //check if there is a spcific document for the team
            var filter = Builders<TeamStasProjection>
                .Filter.And(Builders<TeamStasProjection>.Filter.Eq(x => x.TeamStatistics.Team.Id, response.Team.Id),
                            Builders<TeamStasProjection>.Filter.Eq(x => x.TeamStatistics.League.Season, response.League.Season));

            //create document to store
            var document = new TeamStasProjection
            {
                TeamStatistics = response
            };

            return await base.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = true });
        }

        public async Task<BulkWriteResult<TeamStasProjection>> UpsertAllTeamsStatsAsync(IEnumerable<TeamStatisticsResponse> responses)
        {
            var bulkOperations = new List<WriteModel<TeamStasProjection>>();

            foreach (var response in responses)
            {
                var filter = Builders<TeamStasProjection>
                    .Filter.And(Builders<TeamStasProjection>.Filter.Eq(x => x.TeamStatistics.Team.Id, response.Team.Id),
                                Builders<TeamStasProjection>.Filter.Eq(x => x.TeamStatistics.League.Season, response.League.Season));
                var document = new TeamStasProjection
                {
                    TeamStatistics = response
                };
                var upsertOne = new ReplaceOneModel<TeamStasProjection>(filter, document)
                {
                    IsUpsert = true
                };
                bulkOperations.Add(upsertOne);
            }

            return await base.ReplaceManyAsync(bulkOperations, new BulkWriteOptions { IsOrdered = false });
        }
    }
}
