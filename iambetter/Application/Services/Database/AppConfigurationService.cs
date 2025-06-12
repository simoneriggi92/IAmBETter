using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.Configuration;

namespace iambetter.Application.Services.Database;

public class AppConfigurationService: BaseDataService<AppConfigurationDTO>
{
    public AppConfigurationService(IRepositoryService<AppConfigurationDTO> repositoryService) : base(repositoryService)
    {
    }
}