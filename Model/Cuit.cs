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
        public CuitParameter[] coParameters { get; set; }
        public TemplateVersion[]? coTemplateVersions { get; set; }
    }
    public class ServiceMapper
    {
        public string ivstrWs { get; set; }
        public ServiceMapperItem[] coItems { get; set; }
        public int? ivnumRecLen { get; set; }
        public string? ivstrTemplate { get; set; }
        public string? ivstrSplitter { get; set; }
        public string? ivstrInputType { get; set; }
        public short[] cvnroDocTypes { get; set; }
        public bool? ivblnTaxInLines { get; set; }
        public bool? ivblnCalcPermisoExistente { get; set; }
    }
    public class ServiceMapperItem
    {
        public string? ivstrProperty { get; set; }
        public ServiceMapperItemXPath[]? coXPaths { get; set; }
		public short[]? coStatus { get; set; }  
		public Dictionary<string, string>? coConversion { get; set; }
        public int? ivnumLen { get; set; }
        public string? ivstrLPad { get; set; }
        public string? ivstrRPad { get; set; }
        public string? ivstrformat { get; set; }
        public string? ivstrCoord { get; set; }
        public bool? ivblnRequired { get; set; }
        public string? ivstrDefault { get; set; }

    }
    public class ServiceMapperItemXPath
    {
        public string? ivstrParent { get; set; }
        public string? ivstrData { get; set; }
        public string? ivstrEnum { get; set; }
        public string? ivstrCoord { get; set; }
    }
    public class TemplateVersion
    {
        public short ivnroTipo { get; set; }
        public short ivnroTemplateVersion { get; set; }

    }
    public class CuitParameter
    {
        public string ivstrId { get; set; }
        public string ivstrValue { get; set; }

    }
}