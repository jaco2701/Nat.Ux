using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Applet.Nat.Ux.Models
{
    public class IdentityProviderModel
    {
        public int ivnumIdentityProvider { get; set; }
        public string? ivstrIdentityProvider { get; set; }
        public string? ivstrIdentityProviderId { get; set; }
        public string? ivstrIdentityProviderUrl { get; set; }
        public string? ivstrClientSecret { get; set; }
    }
}