namespace MagazinWPF.Models
{
    /// <summary>
    /// Покупець (клієнт магазину).
    /// Може переглядати товари, додавати в кошик та оформлювати замовлення.
    /// </summary>
    public class Customer : User
    {
        public override string Role => "Customer";

        public override void ShowMenu()
        {
            // TODO: реальну логіку (відкриття каталогу товарів,
            // завантаження кошика з бази тощо) додасть команда бізнес-логіки.
        }
    }
}
