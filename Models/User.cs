namespace MagazinWPF.Models
{
    /// <summary>
    /// Базовий абстрактний клас користувача системи.
    /// Демонструє абстракцію та є основою для наслідування (Admin, Customer).
    /// </summary>
    public abstract class User
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Роль користувача. Кожен похідний клас визначає власне значення.
        /// </summary>
        public abstract string Role { get; }

        /// <summary>
        /// Поліморфний метод: кожна роль може по-своєму "показувати" своє меню.
        /// Реальну логіку (відкриття потрібного вікна, перевірки тощо)
        /// додасть команда, яка реалізує бізнес-логіку.
        /// </summary>
        public abstract void ShowMenu();
    }
}
