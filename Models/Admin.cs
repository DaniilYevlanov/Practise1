namespace MagazinWPF.Models
{
    /// <summary>
    /// Адміністратор магазину.
    /// Може керувати товарами, категоріями та переглядати продажі.
    /// </summary>
    public class Admin : User
    {
        public override string Role => "Admin";

        public override void ShowMenu()
        {
            // TODO: реальну логіку (відкриття панелі адміністратора,
            // перевірки доступу тощо) додасть команда бізнес-логіки.
        }
    }
}
