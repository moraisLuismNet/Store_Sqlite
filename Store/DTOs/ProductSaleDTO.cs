namespace Store.DTOs
{
    public class ProductSaleDTO
    {
        public string ProductName { get; set; } = null!;
        public int Amount { get; set; }
        public decimal? Price { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
