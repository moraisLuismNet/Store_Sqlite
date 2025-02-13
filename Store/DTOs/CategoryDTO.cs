namespace Store.DTOs
{
    public class CategoryDTO
    {
        public int IdCategory { get; set; }

        public string NameCategory { get; set; } = null!;

        public List<ProductDTO> Products { get; set; } = new();
    }
}
