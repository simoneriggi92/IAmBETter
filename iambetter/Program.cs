using iambetter.Application.Services.AI;
using iambetter.Application.Services.API;
using iambetter.Application.Services.Database;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Application.Services.Interfaces;
using iambetter.Domain.Entities.Database.Configuration;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

// Register MongoDB client and database
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return client.GetDatabase(settings.DatabaseName);
});

//add services
builder.Services.AddScoped(typeof(IRepositoryService<>), typeof(MongoRepositoryService<>));
builder.Services.AddScoped<IAIDataSetService, DataSetComposerService>();
builder.Services.AddScoped<BaseDataService<Team>, TeamDataService>();
builder.Services.AddScoped<BaseDataService<MatchProjection>, MatchDataService>();
builder.Services.AddScoped<BaseDataService<TeamStatsProjection>, StatsDataService>();
builder.Services.AddHttpClient<APIDataSetService>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Bind configuration section to settings class

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
