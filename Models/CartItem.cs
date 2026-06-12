namespace MagazinWPF.Models
{
    /// <summary>
    /// Один рядок кошика — товар та його кількість.
    /// </summary>
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public Cart? Cart { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Сума за цим рядком (кількість * ціна за одиницю).
        /// </summary>
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
