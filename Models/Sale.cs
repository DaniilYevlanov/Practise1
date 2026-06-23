namespace MagazinWPF.Models
{

    public class Sale
    {
        public int Id { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public decimal AmountPaid { get; set; }

        public decimal Change { get; set; }

        public string? CashierName { get; set; }

        public int? CartId { get; set; }

        public Cart? Cart { get; set; }

        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    }
}
