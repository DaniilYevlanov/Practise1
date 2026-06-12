namespace MagazinWPF.Models
{
    /// <summary>
    /// Один рядок продажу — товар, кількість і ціна на момент продажу.
    /// </summary>
    public class SaleItem
    {
        public int Id { get; set; }

        public int SaleId { get; set; }

        public Sale? Sale { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Сума за цим рядком продажу.
        /// </summary>
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
