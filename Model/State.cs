using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nat.Ux.Properties;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
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
            ioStatics = new Statics
            {
                coIdentityProviders = [],
                coLists = new List<ListModel>()
            };
            ioUser = null;
        }
        #endregion
        #region PRIVATE PROPS
        private bool livblnStatic0 = false;
        private bool livblnStatic1 = false;
        private IConfiguration mioConfiguration;
        private IdentityProvider _mioIdentityProvider;
        private IdentityProvider mioIdentityProvider
        {
            get
            {
                if (_mioIdentityProvider == null || _mioIdentityProvider.ioDcModel == null)
                    _mioIdentityProvider = new IdentityProvider { ioDcModel = ioStatics.coIdentityProviders.FirstOrDefault(x => x.ivnumIdentityProvider == ivnumIdentityProvider) };
                return _mioIdentityProvider;
            }
            set
            {
                _mioIdentityProvider = value;
            }
        }
        #endregion
        #region PUBLIC PROPS
        public Statics ioStatics { get; set; }
        public bool ivblnLogin { get { return ioUser != null; } }
        public User? ioUser { get; set; }
        public int ivnumInactiveMinutes { get; set; }
        public int ivnumIdentityProvider { get; set; }
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
        #region PUBLIC METHS
        public void SetConfiguration(IConfiguration vioConfiguration)
        {
            mioConfiguration = vioConfiguration;
        }
        public void Login(LoginResponse vioLoginResponse)
        {
            ioUser = vioLoginResponse.ioUser;
            mioIdentityProvider.ivstrToken = vioLoginResponse.ivstrToken;
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
                await mioIdentityProvider.RefreshToken();
            }
        }
        public async Task<string> GetOidcRedirect()
        {
            if (mioIdentityProvider == null || mioIdentityProvider.ioDcModel == null || mioIdentityProvider.ioDcModel.ivnumIdentityProvider == 0)
                throw new Exception(Resources.lioE_NoOidcClients);
            string livstr = Guid.NewGuid().ToString();
            LoginResponse lioLoginResponse = await Fetch<LoginResponse>(
                 EAuthType.OIDC,
                 $"{mioIdentityProvider.ioDcModel.ivnumIdentityProvider}:{mioIdentityProvider.ioDcModel.ivstrIdentityProviderId}:{livstr}",
                 "login",
                 null
            );
            string livstrEnv = ioStatics.coLists.FirstOrDefault(x => x.ivcodType == "FORMAT" && x.ivcodId == "ENV").ivstrDesc == "P" ? string.Empty : "qa";
            mioIdentityProvider.ivstrActualRedirectUrl = $"https://nat{livstrEnv}.signature-services.com.ar/web/authentication/login-callback";
            return mioIdentityProvider.ioDcModel.ivstrIdentityProviderUrlLogin
                    .Replace("{CI}", mioIdentityProvider.ioDcModel.ivstrIdentityProviderId)
                    .Replace("{RU}", Uri.EscapeDataString(mioIdentityProvider.ivstrActualRedirectUrl))
                    .Replace("{ST}", livstr
                    );
        }
        public async Task ProcessCallBack(Uri vioUri)
        {
            Dictionary<string, Microsoft.Extensions.Primitives.StringValues> lcoQParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(vioUri.Query);
            if (lcoQParams.TryGetValue("error", out var lioError))
                throw new Exception($"{Resources.lioE_OIDC}: {lioError}");
            if (!lcoQParams.TryGetValue("code", out var lioCode) || !lcoQParams.TryGetValue("state", out var lioState))
                throw new Exception($"{Resources.lioE_OIDC}: code or state invalid");
            OidcStateModel lioOidcStateModel = await Fetch<OidcStateModel>
            (
                EAuthType.OIDC,
                $"{lioState}",
                 "OidcState",
                 null
            );
            ivnumIdentityProvider = lioOidcStateModel.ivnumIdentityProvider;
            mioIdentityProvider.ivstrState = lioOidcStateModel.ivstrState;
            string livstrEnv = ioStatics.coLists.FirstOrDefault(x => x.ivcodType == "FORMAT" && x.ivcodId == "ENV").ivstrDesc == "P" ? string.Empty : "qa";
            mioIdentityProvider.ivstrActualRedirectUrl = $"https://nat{livstrEnv}.signature-services.com.ar/web/authentication/login-callback";
            await mioIdentityProvider?.GetToken(lioCode);
            JwtSecurityTokenHandler lioJwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jsonToken = lioJwtSecurityTokenHandler.ReadToken(mioIdentityProvider.ivstrToken) as JwtSecurityToken;
            string vivstrUserId = jsonToken?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            Filter lioFilter = new Filter
            {
                coFilterItems =
                [
                    new FilterItem
                    {
                        ivstrOper = "=",
                        ivstrPropName = "ivstrUserEmail",
                        ivstrPropValue = vivstrUserId
                    }
                ]
            };
            User[] lcoUsers = await Post<User[]>(EAuthType.Token, null, "UsersGet", null);
            if (lcoUsers == null || lcoUsers.Length != 1)
                throw new Exception($"{Resources.lioE_OIDCUserNo}[livstruserId]");
            Login(new LoginResponse { ioUser = lcoUsers[0], ivstrToken = mioIdentityProvider.ivstrToken });
        }
        public async Task getStatics(IConfiguration vioConfiguration, short vivnroLevel)
        {
            if (vivnroLevel == 0)
            {
                if (livblnStatic0)
                    return;
                livblnStatic0 = true;
            }
            if (vivnroLevel == 1)
            {
                if (livblnStatic1)
                    return;
                livblnStatic1 = true;
            }
            Statics lioStatics = await Fetch<Statics>(EAuthType.UserAndPass, "0:0", $"Statics{vivnroLevel}", null);
            if (ioStatics == null)
                ioStatics = new Statics { coIdentityProviders = [], coLists = new List<ListModel>() };
            if (lioStatics.coIdentityProviders != null)
            {
                ioStatics.coIdentityProviders = lioStatics.coIdentityProviders;
            }
            foreach (ListModel lioListModel in lioStatics.coLists)
                if (!this.ioStatics.coLists.Any(x => x.ivcodType == lioListModel.ivcodType && x.ivcodId == lioListModel.ivcodId))
                    this.ioStatics.coLists.Add(lioListModel);
            ioStatics.coLists.Add(new ListModel { ivcodId = "0", ivcodType = "TCOMP", ivstrDesc = "Todos" });
            ioStatics.coLists.Add(new ListModel { ivcodId = "0", ivcodType = "STATUS", ivstrDesc = "Todos" });
            if (!int.TryParse(ioStatics.coLists.First(x => x.ivcodType == "EXPIRE" && x.ivcodId == "Session").ivstrDesc ?? string.Empty, null, out int livniumInactiveMinutes))
                livniumInactiveMinutes = 600;
            ivnumInactiveMinutes = livniumInactiveMinutes * 60000;
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
                    return new AuthenticationHeaderValue("NatToken", mioIdentityProvider.ivstrToken);
                case EAuthType.OIDC:
                    return new AuthenticationHeaderValue("NatOIDC", vivstrCredencials);
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
        #endregion
    }
}