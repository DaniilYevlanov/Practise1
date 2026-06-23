namespace MagazinWPF.Models
{
 
    public abstract class User
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;


        public abstract string Role { get; }

        public abstract void ShowMenu();
    }
}
