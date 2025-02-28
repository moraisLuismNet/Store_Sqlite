namespace Store.DTOs
{
    public class ProductDTO
    {
        public int IdProduct { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public DateOnly? DateUp { get; set; }
        public bool Discontinued { get; set; }
        public string? PhotoUrl { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
