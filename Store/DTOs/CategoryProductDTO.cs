namespace Store.DTOs
{
    public class CategoryProductDTO
    {
        public int IdCategory { get; set; }
        public string NameCategory { get; set; }
        public int TotalProducts { get; set; }
        public List<ProductItemDTO> Products { get; set; }
    }
    public class ProductItemDTO
    {
        public int IdProduct { get; set; }
        public string NameProduct { get; set; }
    }
}
