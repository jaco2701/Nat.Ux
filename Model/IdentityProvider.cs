using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Applet.Nat.Ux.Models
{
    public class IdentityProviderModel
    {
        public int ivnumIdentityProvider { get; set; }
        public string? ivstrIdentityProvider { get; set; }
        public string? ivstrIdentityProviderId { get; set; }
        public string? ivstrIdentityProviderUrlLogin { get; set; }
        public string? ivstrIdentityProviderUrlToken { get; set; }
        public string? ivstrClientSecret { get; set; }
    }
    public class IdentityProvider
    {
        public IdentityProviderModel ioDcModel { get; set; }
        public string? ivstrState { get; set; }
        public string? ivstrToken { get; set; }
        public string? ivstrRefreshToken { get; set; }
        public string? ivstrActualRedirectUrl { get; set; }

        public async Task GetToken(string vivstrCode)
        {
            Dictionary<string, string> lcoParams = new Dictionary<string, string>
                {
                    { "client_id", ioDcModel.ivstrIdentityProviderId },
                    { "grant_type", "authorization_code" },
                    { "code", vivstrCode },
                    { "redirect_uri", ivstrActualRedirectUrl },
                    { "client_secret", ioDcModel.ivstrClientSecret }
                };
            using HttpClient lioHttpClient = new HttpClient();
            {
                using HttpResponseMessage lioResponse = await lioHttpClient.PostAsync(ioDcModel.ivstrIdentityProviderUrlToken, new FormUrlEncodedContent(lcoParams));
                {
                    lioResponse.EnsureSuccessStatusCode();
                    TokenResponse lioTokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await lioResponse.Content.ReadAsStringAsync());
                    ivstrToken = lioTokenResponse.AccessToken;
                    ivstrRefreshToken = lioTokenResponse.RefreshToken;
                };
            }
        }
        public async Task RefreshToken()
        {
            if (string.IsNullOrEmpty(ivstrRefreshToken))
            {
                throw new InvalidOperationException("Refresh token is not available.");
            }

            Dictionary<string, string> lcoParams = new Dictionary<string, string>
            {
                { "client_id", ioDcModel.ivstrIdentityProviderId },
                { "grant_type", "refresh_token" },
                { "refresh_token", ivstrRefreshToken },
                { "client_secret", ioDcModel.ivstrClientSecret }
            };

            using HttpClient lioHttpClient = new HttpClient();
            {
                using HttpResponseMessage lioResponse = await lioHttpClient.PostAsync(ioDcModel.ivstrIdentityProviderUrlToken, new FormUrlEncodedContent(lcoParams));
                {
                    if (!lioResponse.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Failed to refresh token. Status code: {lioResponse.StatusCode}");
                    }
                    TokenResponse lioTokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await lioResponse.Content.ReadAsStringAsync());
                    ivstrToken = lioTokenResponse.AccessToken;
                    ivstrRefreshToken = lioTokenResponse.RefreshToken;
                }
            }
        }
    }
    public class OidcRequest
    {
        public string ivstrState { get; set; }
    }
    public class OidcStateModel
    {
        public string ivstrState { get; set; }
        public int ivnumIdentityProvider { get; set; }
        public string ivstrIdentityProviderId { get; set; }
        public DateTime ivdtmState { get; set; }
    }
}