namespace Store.DTOs
{
    public class CategoryDTO
    {
        public int IdCategory { get; set; }

        public string CategoryName { get; set; } = null!;

        public List<ProductDTO> Products { get; set; } = new();

        public int TotalProducts { get; internal set; }
    }
}
