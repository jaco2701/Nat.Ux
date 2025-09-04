using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Applet.Nat.Ux.Models;
using Nat.Ux.Properties;
using Newtonsoft.Json;

namespace Applet.Nat.Ux
{


    public static class Service
    {
        public async static Task<T> Fetch<T>(EAuthType viEAuthType, string vivstrCredencials, IConfiguration vioConfiguration, string vivStrMethod, Object[] vcoParams)
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



        public async static Task<T> Post<T>(EAuthType viEAuthType, string vivstrCredencials, IConfiguration vioConfiguration, string vivStrMethod, Object vioPayload)
        {
           return JsonConvert.DeserializeObject<T>(await PostToStr(viEAuthType, vivstrCredencials, vioConfiguration, vivStrMethod, vioPayload));
        }
        public async static Task<string> PostToStr(EAuthType viEAuthType, string vivstrCredencials, IConfiguration vioConfiguration, string vivStrMethod, Object vioPayload)
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
                throw new Exception("527");
            throw new Exception(Resources.lioE_NoApi);

        }
        public static string GetUrl(IConfiguration vioConfiguration, string vivStrMethod, Object[] vcoParams)
        {
            string livstrUrl = string.Format("{0}{1}",
                vioConfiguration["ivstrUrlbase"].ToString(),
                vioConfiguration["ivstrUrl" + vivStrMethod].ToString()
                );
            if (vcoParams != null && vcoParams.Length > 0)
            {
                for (int livnumParam = 0; livnumParam < vcoParams.Length; livnumParam++)
                    livstrUrl += "/{" + livnumParam.ToString() + "}";
                livstrUrl = string.Format(livstrUrl, vcoParams);
            }
            return livstrUrl;
        }

        private static AuthenticationHeaderValue GetAuthHeader(EAuthType viEAuthType, string vivstrAuthType, string vivstrCredencials)
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



    }
}
