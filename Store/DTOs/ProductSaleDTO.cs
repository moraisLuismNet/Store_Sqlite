namespace Store.DTOs
{
    public class ProductSaleDTO
    {
        public string NameProduct { get; set; } = null!;
        public int Amount { get; set; }
        public decimal? Price { get; set; }
        public string NameCategory { get; set; } = null!;
    }
}
