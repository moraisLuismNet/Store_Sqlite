namespace Store.DTOs
{
    public class ProductInsertDTO
    {
        public string NameProduct { get; set; } = null!;
        public decimal Price { get; set; }
        public DateOnly? DateUp { get; set; }
        public bool Discontinued { get; set; }
        public string? PhotoUrl { get; set; }
        public int? CategoryId { get; set; }
    }
}
