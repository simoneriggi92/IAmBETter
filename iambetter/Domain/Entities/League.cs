namespace iambetter.Domain.Entities
{
    public class League
    {
        public int Id { get; set; }

        public List<Season> Season { get; set; } = new List<Season>();
    }
}
