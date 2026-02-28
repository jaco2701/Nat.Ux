using Applet.Nat.Ux.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
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
        #region CONSTRUCTOR
        public State()
        {
            coLists = new List<ListModel>();
            ioUser = null;
        }
        #endregion
        #region PRIVATE PROPS
        private bool mivblnStatic0 = false;
        private bool mivblnStatic1 = false;
        private IConfiguration mioConfiguration;
        private string mivstrRefreshToken = string.Empty;
        private Dictionary<string, string> mcoIdentityParameter;
        #endregion
        #region PUBLIC PROPS
        public List<ListModel> coLists { get; set; }
        public bool ivblnLogin { get { return ioUser != null; } }
        public User? ioUser { get; set; }
        public int ivnumInactiveMinutes { get; set; }
        public int ivnumIdentityProvider { get; set; }
        public int ivnumBeforeCaptcha
        {
            get
            {
                if (coLists == null)
                    return 1000;
                return int.Parse(GetListParam("EXPIRE", "Captcha"));
            }
        }
        public string? ivstrName { get; set; }
        public string ivstrToken = string.Empty;
        #endregion
        #region PUBLIC METHS
        public void SetConfiguration(IConfiguration vioConfiguration)
        {
            mioConfiguration = vioConfiguration;
        }
        public void Login(LoginResponse vioLoginResponse)
        {
            ioUser = vioLoginResponse.ioUser;
            ivstrToken = vioLoginResponse.ivstrToken ?? string.Empty;
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
        public string GetListParam(string vivCodType, string vivCodId)
        {
            return coLists.FirstOrDefault(x => x.ivcodType.ToUpper() == vivCodType.ToUpper() && x.ivcodId.ToUpper() == vivCodId.ToUpper()).ivstrDesc;
        }
        public List<ListModel> GetListParams(string vivCodType)
        {
            return coLists.Where(x => x.ivcodType == vivCodType.ToUpper()).ToList();
        }
        public async Task LoadStatics()
        {
            if (coLists == null)
            coLists = new List<ListModel>();
            if (!coLists.Any(x => x.ivcodType == "ENV"))
            {
                List<ListModel> lcoLists = await Fetch<List<ListModel>>(EAuthType.UserAndPass, "0:0", $"Statics0", null);
                foreach (ListModel lioListItem in lcoLists)
                    if (!coLists.Any(x => x.ivcodType == lioListItem.ivcodType && x.ivcodId == lioListItem.ivcodId))
                        coLists.Add(lioListItem);
            }
            if (!coLists.Any(x => x.ivcodType == "TCOMP"))
            {
                _ = Fetch<List<ListModel>>(EAuthType.UserAndPass, "0:0", $"Statics1", null).ContinueWith(vioResponse =>
                {
                    List<ListModel> lcoLists = vioResponse.Result;
                    foreach (ListModel lioListItem in lcoLists)
                        if (!coLists.Any(x => x.ivcodType == lioListItem.ivcodType && x.ivcodId == lioListItem.ivcodId))
                            coLists.Add(lioListItem);
                    if (!coLists.Any(x => x.ivcodType == "TCOMP" && x.ivcodId == "0"))
                        coLists.Add(new ListModel { ivcodId = "0", ivcodType = "TCOMP", ivstrDesc = "Todos" });
                    if (!coLists.Any(x => x.ivcodType == "STATUS" && x.ivcodId == "0"))
                        coLists.Add(new ListModel { ivcodId = "0", ivcodType = "STATUS", ivstrDesc = "Todos" });
                    if (!int.TryParse(coLists.First(x => x.ivcodType == "EXPIRE" && x.ivcodId == "Session").ivstrDesc ?? string.Empty, null, out int livniumInactiveMinutes))
                        livniumInactiveMinutes = 600;
                    ivnumInactiveMinutes = livniumInactiveMinutes * 60000;
                });
            }

           
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
        public async Task<T> Fetch<T>(EAuthType viEAuthType, string vivstrCredencials, string vivStrMethod, Object[] vcoParams)
        {
            HttpClient lioHttpClient = new HttpClient();
            lioHttpClient.DefaultRequestHeaders.Authorization = GetAuthHeader(viEAuthType, mioConfiguration["AuthHeader"]?.ToString(), vivstrCredencials);

            lioHttpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
            string livstrUrl = mioConfiguration["Url:Base"]?.ToString() + mioConfiguration[$"Url:{vivStrMethod}"]?.ToString();
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
        public async Task<T> Post<T>(EAuthType viEAuthType, string vivstrCredencials, string vivStrMethod, Object vioPayload)
        {
            return JsonConvert.DeserializeObject<T>(await PostToStr(viEAuthType, vivstrCredencials, vivStrMethod, vioPayload));
        }
        public async Task<string> PostToStr(EAuthType viEAuthType, string vivstrCredencials, string vivStrMethod, Object vioPayload)
        {
            bool livblnRetry = false;
            while (true)
            {
                string livstrResult = await TryPost(viEAuthType, vivstrCredencials, vivStrMethod, vioPayload);
                if (livstrResult != "527")
                    return livstrResult;
                if (livblnRetry)
                    throw new Exception(Resources.lioE_NoApi);
                if (ivnumIdentityProvider <= 0)
                    throw new Exception(Resources.lioE_NoApi);
                livblnRetry = true;
                await RefreshToken();
            }
        }
        public async Task GetToken(string vivstrCode, string vivstrState)
        {
            mcoIdentityParameter = await Post<Dictionary<string, string>>(
                EAuthType.UserAndPass,
                $"0:0",
                "OidcState",
                new OidcRequest
                {
                    ivnroType = (short)eOidcRequestType.GetExchangeParams,
                    ivstrState = vivstrState,
                }
            );
            using (HttpClient lioHttpClient = new HttpClient())
            {
                Dictionary<string, string> lcoExchangeParameter = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", vivstrCode },
                    { "redirect_uri", mcoIdentityParameter["redirect_uri"] },
                    { "client_id", mcoIdentityParameter["client_id"] },
                    { "code_verifier", mcoIdentityParameter["code_verifier"] }
                };
                lcoExchangeParameter["code"] = vivstrCode;
                var lioResponse = await lioHttpClient.PostAsync(mcoIdentityParameter["UrlToken"], new FormUrlEncodedContent(lcoExchangeParameter));
                if (!lioResponse.IsSuccessStatusCode)
                {
                    var errorContent = await lioResponse.Content.ReadAsStringAsync();
                    throw new Exception($"Token exchange failed: {errorContent}");
                }
                TokenResponse lioTokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await lioResponse.Content.ReadAsStringAsync());
                ivstrToken = lioTokenResponse.access_token;
                mivstrRefreshToken = lioTokenResponse.refresh_token;
                ivnumIdentityProvider = int.Parse(mcoIdentityParameter["IdentityProviders"]);
            }
        }
        #endregion
        #region PRIVATE METHS


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
                    return new AuthenticationHeaderValue("NatToken", ivstrToken);
                default:
                    return new AuthenticationHeaderValue("none");
            }
        }
        private async Task<string> TryPost(EAuthType viEAuthType, string vivstrCredencials, string vivStrMethod, Object vioPayload)
        {
            try
            {
                HttpClient lioHttpClient = new HttpClient();
                lioHttpClient.DefaultRequestHeaders.Authorization = GetAuthHeader(viEAuthType, mioConfiguration["AuthHeader"]?.ToString(), vivstrCredencials);

                lioHttpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
                string livstrUrl = mioConfiguration["Url:Base"]?.ToString() + mioConfiguration[$"Url:{vivStrMethod}"]?.ToString();
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

        private async Task RefreshToken()
        {
            if (string.IsNullOrEmpty(mivstrRefreshToken))
            {
                throw new InvalidOperationException("Refresh token is not available.");
            }
            using (HttpClient lioHttpClient = new HttpClient())
            {
                Dictionary<string, string> lcoExchangeParameter = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", mivstrRefreshToken },
                    { "redirect_uri", mcoIdentityParameter["redirect_uri"] },
                    { "client_id", mcoIdentityParameter["client_id"] },
                    { "code_verifier", mcoIdentityParameter["code_verifier"] }
                };
                var lioResponse = await lioHttpClient.PostAsync(mcoIdentityParameter["UrlToken"], new FormUrlEncodedContent(lcoExchangeParameter));
                if (!lioResponse.IsSuccessStatusCode)
                {
                    var errorContent = await lioResponse.Content.ReadAsStringAsync();
                    throw new Exception($"Token exchange failed: {errorContent}");
                }
                TokenResponse lioTokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await lioResponse.Content.ReadAsStringAsync());
                ivstrToken = lioTokenResponse.access_token;
                ivnumIdentityProvider = int.Parse(mcoIdentityParameter["IdentityProviders"]);
            }
        }
        #endregion
    }
}