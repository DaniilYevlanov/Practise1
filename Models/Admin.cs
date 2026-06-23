namespace MagazinWPF.Models
{

    public class Admin : User
    {
        public override string Role => "Admin";

        public override void ShowMenu()
        {

        }
    }
}
