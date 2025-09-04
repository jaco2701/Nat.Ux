using MudBlazor;
using System.Text;
namespace Applet.Nat.Ux.Models
{
    public class State
    {
        #region Private Props
        #endregion
        #region Public Props
        public event System.Action OnChange;
        public Statics? ioStatics { get; set; }
        public bool ivblnLogin { get; set; }
        public bool ivblnReady { get; set; }
        public LoginResponse ioLogin { get; set; }
        public int ivnumInactiveMinutes { get; set; }
        public int ivnumBeforeCaptcha
        {
            get
            {
                if (ioStatics == null || ioStatics.coLists == null)
                    return 1000;
                return int.Parse(ioStatics.coLists.First(x => x.ivcodType == "EXPIRE" && x.ivcodId == "Captcha").ivstrDesc);
            }
        }
        public string? ivstrName { get; set; }
        #endregion
        #region Private Meths
        private void NotifyStateChanged() => OnChange?.Invoke();
        #endregion
        #region Public Meths
        public void Logout()
        {
            ivblnLogin = false;
            ioLogin = null;
        }
        public void reset()
        {
            ivblnLogin = false;
        }
        public async Task getStatics(IConfiguration vioConfiguration)
        {
            if (this.ioStatics == null)
            {
                this.ioStatics = await Service.Fetch<Statics>(EAuthType.UserAndPass, "0:0", vioConfiguration, "Statics", null);
                ioStatics.coLists.Add(new ListModel { ivcodId = "0", ivcodType = "TCOMP", ivstrDesc = "Todos" });
                ioStatics.coLists.Add(new ListModel { ivcodId = "0", ivcodType = "STATUS", ivstrDesc = "Todos" });
                if (!int.TryParse(ioStatics.coLists.First(x => x.ivcodType == "EXPIRE" && x.ivcodId == "Session").ivstrDesc ?? string.Empty, null, out int livniumInactiveMinutes))
                   livniumInactiveMinutes=600;
                ivnumInactiveMinutes = livniumInactiveMinutes * 60000;
            }
        }
        public string Getparam(string vivCodType, string vivCodId)
        {
            return ioStatics.coLists.FirstOrDefault(x => x.ivcodType == vivCodType.ToUpper() && x.ivcodId == vivCodId).ivstrDesc;
        }
        public Encoding GetEncoding()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); 
            string livstr=ioLogin.ioUser.coCuitsModels.FirstOrDefault(x=>x.ivlngCuit==ioLogin.ivlngCurrentCuit)?.ivstrEncoding??string.Empty;
            if (string.IsNullOrEmpty(livstr))
                return Encoding.UTF8;
            if (int.TryParse(livstr, out int livnum))
                return Encoding.GetEncoding(livnum);
            return Encoding.GetEncoding(livstr);
        }
        #endregion
    }
}