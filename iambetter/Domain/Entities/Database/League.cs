namespace iambetter.Domain.Entities.Database
{
    public class League
    {
        public int Id { get; set; }

        public List<Season> Season { get; set; } = new List<Season>();
    }
}
