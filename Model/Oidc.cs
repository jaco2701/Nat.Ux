
namespace Applet.Nat.Ux.Models
{
    public class OidcRequest
    {
        public short ivnroType { get; set; } 
        public int ivnumIdentityProvider { get; set; } = 0;
        public string? ivstrState { get; set; }
        public string? ivstrCode { get; set; }
        public string? ivstrToken { get; set; }
    }
}