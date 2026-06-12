namespace MagazinWPF.Models
{
    /// <summary>
    /// Товар у каталозі магазину.
    /// </summary>
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string? ImagePath { get; set; }

        public string? Barcode { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsTop { get; set; }

        public bool IsNew { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
