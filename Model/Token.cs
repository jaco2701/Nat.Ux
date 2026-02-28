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
        public string? ivstrRefreshToken { get; set; }

    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
        public int expires_in { get; set; }
    }

}
