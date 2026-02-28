using Newtonsoft.Json;

namespace Applet.Nat.Ux.Models
{
    public class Document
    {
        public EDcOper eDcOper { get; set; }
        public DocumentModel? ioDcModel { get; set; }
        public string? ivstrKey { get; set; }
    }
    public class DocumentModel
    {
        public long ivlngDoc { get; set; }
        public long ivlngCuitEmisor { get; set; }
        public short ivnroTipo { get; set; }
        public int ivnumPvta { get; set; }
        public long ivlngCbte { get; set; }
        public DateTime? ivdtmEmision { get; set; }
        public long ivlngCuitReceptor { get; set; }
        public string? ivstrWs { get; set; }
        public string? ivstrInData { get; set; }
        public double ivdblImporte { get; set; }
        public short ivnroStatus { get; set; }
        public string? ivstrInType { get; set; }
        public string? ivstrIdCliente { get; set; }
        public string? ivstrMoneda { get; set; }
        public string? ivstrRazonSocial { get; set; }
    }
    public class DocumentTask
    {
        public long[]? cvlngDocs { get; set; }
        [JsonProperty("Task")]
        public eTask ieTask { get; set; }
    }
    public class DocumentKey
    {
        [JsonProperty("CuitEmisor")]
        public long ivlngCuitEmisor { get; set; }

        [JsonProperty("Tipo")]
        public short ivnroTipo { get; set; }

        [JsonProperty("PuntoDeVenta")]
        public int ivnumPvta { get; set; }

        [JsonProperty("Numero")]
        public long ivlngCbte { get; set; }
    }

    public class DocumentTaskResponse
    {
        [JsonProperty("Id.Nat")]
        public long ivlngDoc { get; set; }
        [JsonProperty("Respuesta")]
        public string? ioData { get; set; }
        [JsonProperty("Documento")]
        public DocumentKey? ioKey { get; set; }
        public string? ivstrKey { get { return $"{ioKey?.ivlngCuitEmisor}_{ioKey?.ivnroTipo}_{ioKey?.ivnumPvta}_{ioKey?.ivlngCbte}_{ivlngDoc}"; } }
    }


    public class DocumentTracking
    {
        public DocumentTrackingModel ioDcModel { get; set; }
        public string ivstrTrackDescr { get; set; }
        public bool ivblnShowData
        {
            get
            {
                if (mivblnShowData == null)
                    mivblnShowData = false;
                return mivblnShowData?? false;
            }
            set { 
                mivblnShowData = value;
            }
        }
        private bool? mivblnShowData;
    }
    public class DocumentTrackingModel
    {
        public DateTime ivdtmTrack { get; set; }
        public short ivnroStatus { get; set; }
        public string? ivstrData { get; set; }
        public string? ivstrwsMethod { get; set; }
        public int ivnumtrack { get; set; }
    }
    public class DocumentUploadRequest
    {
        [JsonProperty("Documento")]
        public string? ivstrData { get; set; }
        [JsonProperty("Nombre")]
        public string? ivstrName { get; set; }
        [JsonProperty("Comp")]
        public bool? ivblnComp { get; set; }
        [JsonProperty("CuitEmisor")]
        public long ivlngCuit { get; set; }
    }
    public class DocumentUploadResponse
    {
        [JsonProperty("Id.Nat")]
        public long? ivlngDoc { get; set; }
        [JsonProperty("CuitEmisor")]
        public long? ivlngCuitEmisor { get; set; }
        [JsonProperty("PuntoDeVenta")]
        public int? ivnumPvta { get; set; }
        [JsonProperty("Tipo")]
        public short? ivnroTipoDoc { get; set; }
        [JsonProperty("Numero")]
        public long? ivlngCbte { get; set; }
        [JsonProperty("Estado")]
        public short? ivnroStatus { get; set; }
        [JsonProperty("Descripcion")]
        public string? ivstrDescStatus { get; set; }
    }
}

