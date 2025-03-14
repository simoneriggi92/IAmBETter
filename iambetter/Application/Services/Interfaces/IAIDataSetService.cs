using iambetter.Domain.Entities.Database.Projections;

namespace iambetter.Application.Services.Interfaces
{
    public interface IAIDataSetService
    {
        public void GenerateDataSet(IEnumerable<TeamStatsProjection> stats);
        public void WriteCsv<T>(string filePath, List<T> records);
    }
}
