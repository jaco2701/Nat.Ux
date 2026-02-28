using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Applet.Nat.Ux.Models
{
    public class ListModel
    {
        public string ivcodType { get; set; }
        public string ivcodId { get; set; }
        public string ? ivstrDesc { get; set; }
    }
    public class List
    {
        public ListModel ioDcModel { get; set; }
        public eTask ieTask { get; set; }
    }
}