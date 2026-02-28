using Newtonsoft.Json;

namespace Applet.Nat.Ux.Models
{ 
    public class DocumentUser
    {
        public long? ivlngCuitEmisor { get; set; }
        public short? ivnroTipoDocReceptor { get; set; }
        public long? ivlngDocReceptor { get; set; }
        public short? ivnroTipoResp { get; set; }
        public int? ivnumPvta { get; set; }
        public short? ivnroTipoDoc { get; set; }
        public int? ivlngCbte { get; set; }
        public short? ivnroConcepto { get; set; }
        public string? ivdtmEmision { get; set; }
        public string? ivdtmServdesde { get; set; }
        public string? ivdtmServhasta { get; set; }
        public string? ivdtmVtopago { get; set; }
        public double? ivdblImporteTotal { get; set; }
        public double? ivdblImporteGravado { get; set; }
        public double? ivdblImporteNoGravado { get; set; }
        public double? ivdblImporteExento { get; set; }
        public double? ivdblImporteOtrosTributos { get; set; }
        public double? ivdblImporteIva { get; set; }
        public string? ivstrMoneda { get; set; }
        public double? ivdblCotizacion { get; set; }
        public string? ivstrRazonSocial { get; set; }
        public string? ivstrCondPago { get; set; }
        public string? ivstrObs { get; set; }
        public UxDomicilio? ioDomicilio { get; set; }
        public string? ivstrWs { get; set; }
        public List<UxDocumentAsociado>? coAsociados { get; set; }
        public List<UxDocumentOtroTributo>? coOtrosTributos { get; set; }
        public List<UxDocumentIva>? coIvas { get; set; }
        public List<UxDocumentOpcional>? coOpcionales { get; set; }
        public List<UxDocumentAdicional>? coAdicionales { get; set; }
        public List<UxDocumentComprador>? coCompradores { get; set; }
        public List<UxDocumentItem>? coItems { get; set; }
        public List<UxDocumentItemCT>? coItemsCT { get; set; }
        public UxAuth? ioAuth { get; set; }
    }
    public class UxDomicilio
    {
        public string? ivstrCalle { get; set; }
        public string? ivstrNro { get; set; }
        public string? ivstrPiso { get; set; }
        public string? ivstrDepto { get; set; }
        public string? ivstrCuidad { get; set; }
        public short? ivnroPcia { get; set; }
        public string? ivstrPais { get; set; }
    }
    public class UxDocumentAsociado
    {
        #region Propiedades
        public short? ivnroCbtetipo { get; set; }
        public short? ivnroCbtePuntovta { get; set; }
        public long? ivlngCbteNro { get; set; }
        public long? ivlngCbteCUIT { get; set; }
        public string? ivdtmFechaEmision { get; set; }
        #endregion
    }
    public class UxDocumentOtroTributo
    {
        #region Propiedades
        public short? ivnroId { get; set; }
        public string? ivstrDesc { get; set; }
        public Double? ivdblBaseImponible { get; set; }
        public Double? ivdblAlicuota { get; set; }
        public Double? ivdblImporte { get; set; }
        #endregion
    }
    public class UxDocumentIva
    {
        #region Propiedades
        public short? ivnroTipo { get; set; }
        public Double? ivdblBaseImponible { get; set; }
        public Double? ivdblImporte { get; set; }
        #endregion
    }
    public class UxDocumentOpcional
    {
        #region Propiedades
        public string? ivstrId { get; set; }
        public string? ivstrValor { get; set; }
        #endregion
    }
    public class UxDocumentAdicional
    {
        #region Propiedades
        public short? ivnroTipo { get; set; }
        public string? ivstrValor1 { get; set; }
        public string? ivstrValor2 { get; set; }
        public string? ivstrValor3 { get; set; }
        public string? ivstrValor4 { get; set; }
        public string? ivstrValor5 { get; set; }
        #endregion
    }
    public class UxDocumentComprador
    {
        #region Propiedades
        public short? ivnroDocTipo { get; set; }
        public long? ivlngDocNro { get; set; }
        public Double? ivdblPorcentaje { get; set; }
        #endregion
    }
    public class UxDocumentItem
    {
        #region Propiedades
        public string? ivstrDescripcion { get; set; }
        public double? ivdblCantidad { get; set; }
        public double? ivdblPrecioUnitario { get; set; }
        public double? ivdblBonificaion { get; set; }
        public int? ivnroUM { get; set; }
        public double? ivdblImporteTotal { get; set; }
        public short? ivnroTipoIVA { get; set; }
        public double? ivdblImporteIVA { get; set; }
        #endregion
    }
    public class UxDocumentItemCT
    {
        #region Propiedades
        public short? ivnroTipo { get; set; }
        public short? ivnroCodigoTurismo { get; set; }
        public string? ivstrCodigo { get; set; }
        public string? ivstrDesc { get; set; }
        public short? ivnroTipoIVA { get; set; }
        public double? ivdblImporteItem { get; set; }
        #endregion
    }
    public class UxAuth
    {
        #region Propiedades
        public string? ivstrAuthCode { get; set; }
        public string? ivdtmAuthVenc { get; set; }
        #endregion
    }


}