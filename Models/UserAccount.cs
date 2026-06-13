using System.Security.Cryptography;
using System.Text;

namespace MagazinWPF.Models
{
    /// <summary>
    /// Запис користувача у базі даних.
    /// Зберігає логін, хеш пароля та роль (Admin / Customer).
    /// </summary>
    public class UserAccount
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Значення: "Admin" або "Customer".
        /// </summary>
        public string Role { get; set; } = "Customer";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Допоміжні методи ──────────────────────────────────────────

        public static string HashPassword(string plainPassword)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainPassword));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        public bool VerifyPassword(string plainPassword)
            => PasswordHash == HashPassword(plainPassword);

        /// <summary>
        /// Перетворює запис БД на доменний об'єкт User (Admin або Customer).
        /// </summary>
        public User ToUser() => Role == "Admin"
            ? new Admin   { Login = Login, FullName = FullName }
            : new Customer { Login = Login, FullName = FullName };
    }
}
