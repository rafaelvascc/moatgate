namespace MoatGate.Models.Logout
{
    public class LogoutViewModel
    {
        public string LogoutId { set; get; }
        //yes or no
        public string ConfirmLogout { set; get; } 
        public string RedirectUrl { set; get; }
    }
}
