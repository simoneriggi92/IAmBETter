﻿namespace iambetter.Domain.Entities.Database.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string LeagueCollectionName { get; set; }
        public string TeamCollectionName { get; set; }
        public string MatchCollectionName { get; set; }
        public string StatsCollectionName { get; set; }
        public string TaskCollectionName { get; set; }
        public string PredictionsCollectionName { get; set; }
        public string LeagueInfoCollectionName { get; set; }
        public string PredictionsHistoryCollectionName { get; set; }
        
        public string ConfigurationCollectionName { get; set; }
    }
}
