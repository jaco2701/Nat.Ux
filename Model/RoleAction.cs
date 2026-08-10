using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Applet.Nat.Ux.Models
{
    public class RoleAction
    {
        public RoleActionModel ioDcModel { get; set; }
        public eTask ieTask { get; set; }
    }
    public class RoleActionModel
    {
        public short? ivnroRole { get; set; }
        public short? ivnroAction { get; set; }
    }
    public class RoleActionUx
    {
        public short? ivnroRole { get; set; }
        public short? ivnroAction { get; set; }
        public string? ivstrEntity { get; set; }
        public string? ivstrAction { get; set; }
        public bool ivblnSelected { get; set; }
    }



}