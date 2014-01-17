namespace ApplicationSource.Models
{
    public class UserAuthenticationModel
    {
        public bool IsAuthenticated { get; set; }
        public string CookieName { get; set; }
        public string EncryptedTicket { get; set; }
        public string ErrorMessage { get; set; }

    }
}