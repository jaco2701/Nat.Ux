using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Applet.Nat.Ux.Models
{
    public class Cuit
    {
        public CuitModel? ioDcModel { get; set; }
        public eTask ieTask { get; set; }
        public CuitCnfg? ioCnfg { get; set; }

    }
    public class CuitModel
    {
        public long ivlngCuit { get; set; }
        public string? ivstrCnfg { get; set; }
        public string? ivstrCuitRS { get; set; }

    }
    public class CuitCnfg
    {
        public ServiceMapper[]? coServiceMappers { get; set; }
        public string? ivstrInFolder { get; set; }
        public string? ivstrOutFolder { get; set; }
        public bool? ivblnAdmin { get; set; }
        public TemplateVersion[]? coTemplateVersions { get; set; }
        public string? ivstrEncoding { get; set; }
    }
    public class ServiceMapper
    {
        public string ivstrWs { get; set; }
        public ServiceMapperItem[] coItems { get; set; }
        public int? ivnumRecLen { get; set; }
        public string? ivstrTemplate { get; set; }
        public string? ivstrInputType { get; set; }
        public short[] cvnroDocTypes { get; set; }
        
    }
    public class ServiceMapperItem
    {
        public string? ivstrProperty { get; set; }
        public ServiceMapperItemXPath[]? coXPaths { get; set; }
        public int? ivnumLen { get; set; }
        public string? ivstrLPad { get; set; }
        public string? ivstrRPad { get; set; }
        public string? ivstrformat { get; set; }
        public string? ivstrCoord { get; set; }

    }
    public class ServiceMapperItemXPath
    {
        public string? ivstrParent { get; set; }
        public string? ivstrData { get; set; }
        public string? ivstrEnum { get; set; }
    }
    public class TemplateVersion
    {
        public short ivnroTipo { get; set; }
        public short ivnroTemplateVersion { get; set; }

    }
}