namespace MagazinWPF.Models
{
    /// <summary>
    /// Кошик покупця.
    /// </summary>
    public class Cart
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Active";

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        /// <summary>
        /// Загальна сума кошика (обчислюється автоматично за товарами в ньому).
        /// </summary>
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }
}
