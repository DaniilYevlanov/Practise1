namespace MagazinWPF.Models
{
   
    public class Customer : User
    {
        public override string Role => "Customer";

        public override void ShowMenu()
        {
           
        }
    }
}
