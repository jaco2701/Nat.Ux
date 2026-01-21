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
        public string ivstrToken { get; set; }

    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
