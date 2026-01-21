using Nat.Ux.Properties;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
namespace Applet.Nat.Ux.Models
{
    public class State
    {
        #region Constructor
        public State()
        {
            ioStatics = new Statics
            {
                coIdentityProviders = [new IdentityProviderModel { ivnumIdentityProvider = 0, ivstrIdentityProvider = "Nat" }],
                coLists = new List<ListModel>()
            };
            reset();
        }
        #endregion
        #region Private Props
        #endregion
        #region Public Props
        public event System.Action? OnChange;
        public Statics ioStatics { get; set; }
        public bool ivblnLogin { get { return ioUser != null; } }
        public bool ivblnReady { get; set; }
        public User? ioUser { get; set; }
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
        public IdentityProvider ioIdentityProvider { get; set; }
        #endregion
        #region Public Meths
        public void Login(LoginResponse vioLoginResponse)
        {
            ioUser = vioLoginResponse.ioUser;
            ioIdentityProvider.ivstrToken = vioLoginResponse.ivstrToken;
            if (ioUser.coCuitsModels == null || ioUser.coCuitsModels.Count == 0)
                ioUser.ivlngCurrentCuit = 0;
            else
            {
                ioUser.ivlngCurrentCuit = ioUser.coCuitsModels[0].ivlngCuit;
                foreach (UserCuitModel lioO in ioUser.coCuitsModels)
                    if (lioO.ivblnDefaut)
                    {
                        ioUser.ivlngCurrentCuit = lioO.ivlngCuit;
                        break;
                    }
            }
        }
        public void Logout()
        {
            ioUser = null;
            ioIdentityProvider = null;
        }
        public void reset()
        {
            ioUser = null;
            ioIdentityProvider = new IdentityProvider { ioDcModel = ioStatics.coIdentityProviders[0] };
        }
        public async Task getStatics(IConfiguration vioConfiguration)
        {
            if (this.ioStatics.coLists.Count() == 0)
            {
                this.ioStatics = await Fetch<Statics>(EAuthType.UserAndPass, "0:0", vioConfiguration, "Statics", null);
                ioStatics.coLists.Add(new ListModel { ivcodId = "0", ivcodType = "TCOMP", ivstrDesc = "Todos" });
                ioStatics.coLists.Add(new ListModel { ivcodId = "0", ivcodType = "STATUS", ivstrDesc = "Todos" });
                if (!int.TryParse(ioStatics.coLists.First(x => x.ivcodType == "EXPIRE" && x.ivcodId == "Session").ivstrDesc ?? string.Empty, null, out int livniumInactiveMinutes))
                    livniumInactiveMinutes = 600;
                ivnumInactiveMinutes = livniumInactiveMinutes * 60000;
                NotifyStateChanged();
            }
        }
        public string Getparam(string vivCodType, string vivCodId)
        {
            return ioStatics.coLists.FirstOrDefault(x => x.ivcodType == vivCodType.ToUpper() && x.ivcodId == vivCodId).ivstrDesc;
        }
        public Encoding GetEncoding()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string livstr = ioUser.coCuitsModels.FirstOrDefault(x => x.ivlngCuit == ioUser.ivlngCurrentCuit)?.ivstrEncoding ?? string.Empty;
            if (string.IsNullOrEmpty(livstr))
                return Encoding.UTF8;
            if (int.TryParse(livstr, out int livnum))
                return Encoding.GetEncoding(livnum);
            return Encoding.GetEncoding(livstr);
        }
        public async Task<T> Fetch<T>(EAuthType viEAuthType, string vivstrCredencials, IConfiguration vioConfiguration, string vivStrMethod, Object[] vcoParams)
        {
            HttpClient lioHttpClient = new HttpClient();
            lioHttpClient.DefaultRequestHeaders.Authorization = GetAuthHeader(viEAuthType, vioConfiguration["AuthHeader"]?.ToString(), vivstrCredencials);

            lioHttpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
            string livstrUrl = vioConfiguration["Url:Base"]?.ToString() + vioConfiguration[$"Url:{vivStrMethod}"]?.ToString();
            if (vcoParams != null && vcoParams.Length > 0)
            {
                for (int livnumParam = 0; livnumParam < vcoParams.Length; livnumParam++)
                    livstrUrl += "/{" + livnumParam.ToString() + "}";
                livstrUrl = string.Format(livstrUrl, vcoParams);
            }
            var lioreq = await lioHttpClient.GetAsync(livstrUrl);
            if (lioreq.IsSuccessStatusCode)
            {
                Response lioResponse = await lioreq.Content.ReadFromJsonAsync<Response>();
                if (lioResponse == null)
                    throw new Exception(Resources.lioE_NoApi);
                if (lioResponse.ivnumStatus != 0)
                {
                    ResponseException lioResponseException = JsonConvert.DeserializeObject<ResponseException>(lioResponse.ioData.ToString());
                    throw new Exception(lioResponseException.ivstrMsg);
                }
                return JsonConvert.DeserializeObject<T>(lioResponse.ioData.ToString());
            }
            if (lioreq.StatusCode == (HttpStatusCode)527)
                throw new Exception("527");
            throw new Exception(Resources.lioE_NoApi);
        }
        public async Task<T> Post<T>(EAuthType viEAuthType, string vivstrCredencials, IConfiguration vioConfiguration, string vivStrMethod, Object vioPayload)
        {
            return JsonConvert.DeserializeObject<T>(await PostToStr(viEAuthType, vivstrCredencials, vioConfiguration, vivStrMethod, vioPayload));
        }
        public async Task<string> PostToStr(EAuthType viEAuthType, string vivstrCredencials, IConfiguration vioConfiguration, string vivStrMethod, Object vioPayload)
        {
            bool livblnRetry = false;
            while (true)
            {
                string livstrResult = await TryPost(viEAuthType, vivstrCredencials, vioConfiguration, vivStrMethod, vioPayload);
                if (livstrResult != "527")
                    return livstrResult;
                if (livblnRetry)
                    throw new Exception(Resources.lioE_NoApi);
                if (ioIdentityProvider == null)
                    throw new Exception(Resources.lioE_NoApi);
                livblnRetry = true;
                await ioIdentityProvider.RefreshToken();
            }
        }
        #endregion
        #region Private Meths
        private void NotifyStateChanged() => OnChange?.Invoke();
        private AuthenticationHeaderValue GetAuthHeader(EAuthType viEAuthType, string vivstrAuthType, string vivstrCredencials)
        {
            switch (viEAuthType)
            {
                case EAuthType.UserAndPass:
                    return new AuthenticationHeaderValue(
                        vivstrAuthType,
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes(
                                vivstrCredencials
                                )
                            )
                        );
                case EAuthType.Token:
                    return new AuthenticationHeaderValue("NatToken", vivstrCredencials);
                default:
                    return new AuthenticationHeaderValue("none");
            }
        }
        private async Task<string> TryPost(EAuthType viEAuthType, string vivstrCredencials, IConfiguration vioConfiguration, string vivStrMethod, Object vioPayload)
        {
            try
            {
                HttpClient lioHttpClient = new HttpClient();
                lioHttpClient.DefaultRequestHeaders.Authorization = GetAuthHeader(viEAuthType, vioConfiguration["AuthHeader"]?.ToString(), vivstrCredencials);

                lioHttpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
                string livstrUrl = vioConfiguration["Url:Base"]?.ToString() + vioConfiguration[$"Url:{vivStrMethod}"]?.ToString();
                HttpResponseMessage lioHttpResponseMessage = await lioHttpClient.PostAsync(
                    livstrUrl,
                    new StringContent(JsonConvert.SerializeObject(vioPayload), Encoding.UTF8, "application/json")
                );
                if (lioHttpResponseMessage.IsSuccessStatusCode)
                {
                    Response lioResponse = await lioHttpResponseMessage.Content.ReadFromJsonAsync<Response>();
                    if (lioResponse == null)
                        throw new Exception(Resources.lioE_NoApi);
                    if (lioResponse.ivnumStatus != 0)
                    {
                        ResponseException lioResponseException = JsonConvert.DeserializeObject<ResponseException>(lioResponse.ioData.ToString());
                        throw new Exception(lioResponseException.ivstrMsg + ": " + lioResponseException.ivstrStack);
                    }
                    return lioResponse.ioData.ToString();
                }
                if (lioHttpResponseMessage.StatusCode == (HttpStatusCode)527)
                    return "527";
                throw new Exception(Resources.lioE_NoApi);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception(Resources.lioE_NoApi, ex);
            }
        }
        #endregion 
    }
}