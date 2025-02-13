namespace Store.DTOs
{
    public class ProductUpdateDTO
    {
        public int IdProduct { get; set; }
        public string NameProduct { get; set; } = null!;
        public decimal Price { get; set; }
        public DateOnly? DateUp { get; set; }
        public bool Discontinued { get; set; }
        public string? PhotoUrl { get; set; }
        public int? CategoryId { get; set; }
    }
}
