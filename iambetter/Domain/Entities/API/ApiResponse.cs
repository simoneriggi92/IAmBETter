namespace iambetter.Domain.Entities.Models
{
    public class ApiResponse<T>
    {
        public List<T> Response { get; set; } = new();
    }
}
