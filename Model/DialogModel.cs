namespace Applet.Nat.Ux.Models
{
    public class DialogModel
    {
        public eDialogType? ieDialogType { get; set; }
        public eDialogButtoms? ieDialogButtoms { get; set; }
        public string? ivstrBody { get; set; }
        public string? ivstrClass { get; set; }
        public DocumentTaskResponse[]? coResponses { get; set; }
        public DocumentTracking[]? coDocumentTracking { get; set; }
        public List<DocumentUploadRequest>? coDocumentsUpload { get; set; }
        public User? ioUser { get; set; }
        public Cuit? ioCuit { get; set; }
        public CuitCuit? ioCuitCuit { get; set; }
        public List? ioList { get; set; }
        public bool ivblnOK { get; set; }
        public string? ivstrFullBody { get; set; }
    }
}
