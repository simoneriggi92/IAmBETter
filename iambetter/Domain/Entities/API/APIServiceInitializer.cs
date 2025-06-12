using iambetter.Application.Services.API;

namespace iambetter.Domain.Entities.API;

public class APIServiceInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public APIServiceInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var apiService = scope.ServiceProvider.GetRequiredService<APIService>();
        await apiService.InitializeAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}