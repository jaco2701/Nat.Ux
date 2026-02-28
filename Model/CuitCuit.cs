using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Applet.Nat.Ux.Models
{
    public class CuitCuit
    {
        public CuitCuitModel? ioDcModel { get; set; }
        public eTask ieTask { get; set; }

    }
    public class CuitCuitModel
    {
        public long ivlngCuit { get; set; }
        public long ivlngCuitReceptor { get; set; }
        public string? ivstrEmail { get; set; }
        public string? ivstrRS { get; set; }
        
    }
}