namespace Store.DTOs
{
    public class CategoryProductDTO
    {
        public int IdCategory { get; set; }
        public string CategoryName { get; set; }
        public int TotalProducts { get; set; }
        public List<ProductItemDTO> Products { get; set; }
    }
    public class ProductItemDTO
    {
        public int IdProduct { get; set; }
        public string ProductName { get; set; }
    }
}
