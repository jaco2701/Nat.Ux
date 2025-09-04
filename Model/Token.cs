namespace Applet.Nat.Ux.Models
{
    public class TokenUser
    {
        public long? ivlngCuit { get; set; }
        public string? ivstrUser { get; set; }
        public long? ivlngUser { get; set; }
    }


    public class LoginResponse
    {
        public User ioUser { get; set; }
        public string? ivstrToken { get; set; }
        public long ivlngCurrentCuit { get; set; }

    }
}
