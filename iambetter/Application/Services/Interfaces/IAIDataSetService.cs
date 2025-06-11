using iambetter.Domain.Entities.AI;
using iambetter.Domain.Entities.Database.Projections;

namespace iambetter.Application.Services.Interfaces
{
    public interface IAIDataSetService
    {
        public void GenerateDataSet(IEnumerable<MatchDTO> matches);
        string GetCsvFilePath();
        public void WriteCsv(string filePath, List<MatchRecord> records);
    }
}
