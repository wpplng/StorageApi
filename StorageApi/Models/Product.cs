namespace StorageApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string Category { get; set; } = null!;
        public string Shelf { get; set; } = null!;
        public int Count { get; set; }
        public string Description { get; set; } = null!; 

    }
}
