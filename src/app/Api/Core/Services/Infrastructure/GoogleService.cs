using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Services.Interfaces.Infrastructure;
using Api.Core.Settings;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Options;

namespace Api.Core.Services.Infrastructure
{
    public class GoogleService : IGoogleService
    {
        private readonly GoogleSettings _googleSettings;

        public GoogleService(IOptions<GoogleSettings> googleSettings)
        {
            _googleSettings = googleSettings.Value;
        }

        public string GetOAuthUrl()
        {
            var url = CreateFlow()
                .CreateAuthorizationCodeRequest(_googleSettings.RedirectUrl)
                .Build()
                .AbsoluteUri;

            return url;
        }

        public async Task<GooglePayloadModel> ExchangeCodeForTokenAsync(string code)
        {
            var tokenResponse = await CreateFlow()
                .ExchangeCodeForTokenAsync(null, code, _googleSettings.RedirectUrl, CancellationToken.None);

            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new List<string> {_googleSettings.ClientId}
                });

            return payload != null
                ? new GooglePayloadModel
                {
                    Email = payload.Email,
                    FamilyName = payload.FamilyName,
                    GivenName = payload.GivenName
                }
                : null;
        }

        private GoogleAuthorizationCodeFlow CreateFlow()
        {
            return new(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _googleSettings.ClientId,
                    ClientSecret = _googleSettings.ClientSecret
                },
                Scopes = new List<string> { "email", "profile" },
                IncludeGrantedScopes = true
            });
        }
    }
}
