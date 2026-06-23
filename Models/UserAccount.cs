using System.Security.Cryptography;
using System.Text;

namespace MagazinWPF.Models
{

    public class UserAccount
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

 
        public string Role { get; set; } = "Customer";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;



        public static string HashPassword(string plainPassword)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainPassword));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        public bool VerifyPassword(string plainPassword)
            => PasswordHash == HashPassword(plainPassword);


        public User ToUser() => Role == "Admin"
            ? new Admin   { Login = Login, FullName = FullName }
            : new Customer { Login = Login, FullName = FullName };
    }
}
