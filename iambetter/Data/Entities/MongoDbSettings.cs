namespace iambetter.Data.Entities
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string LeagueCollectionName { get; set; }
        public string TeamCollectionName { get; set; }
        public string MatchCollectionName { get; set; }
    }
}
