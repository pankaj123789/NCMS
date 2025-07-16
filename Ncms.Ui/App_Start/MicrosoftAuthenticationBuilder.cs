using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Ncms.Bl;
using Ncms.Contracts;
using Ncms.Ui;
using Owin;

namespace Ncms.Ui
{
    public static class MicrosoftAuthenticationBuilder
    {
        // RedirectUri is the URL where the user will be redirected to after they sign in.
        static string redirectUri = System.Configuration.ConfigurationManager.AppSettings["AzureAppRedirectUri"];

        public static void Configure(IAppBuilder app)
        {
            var secretsProvider = DependencyResolver.Current.GetService<ISecretsCacheQueryService>();

            // The Client ID is used by the application to uniquely identify itself to Azure AD.
            var clientId = secretsProvider.Get("AzureAppClientId");

            // Tenant is the tenant ID (e.g. contoso.onmicrosoft.com, or 'common' for multi-tenant)
            var tenant = secretsProvider.Get("AzureAppTenant");

            // Authority is the URL for authority, composed by Microsoft identity platform endpoint and the tenant name (e.g. https://login.microsoftonline.com/contoso.onmicrosoft.com/v2.0)
            var authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["AzureAppAuthority"], tenant);

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOpenIdConnectAuthentication(
            new OpenIdConnectAuthenticationOptions
                {
                    // Sets the ClientId, authority, RedirectUri as obtained from web.config
                    ClientId = clientId,
                    Authority = authority,
                    RedirectUri = redirectUri,
                    // PostLogoutRedirectUri is the page that users will be redirected to after sign-out. In this case, it is using the home page
                    PostLogoutRedirectUri = redirectUri,
                    Scope = OpenIdConnectScope.OpenIdProfile,
                    // ResponseType is set to request the id_token - which contains basic information about the signed-in user
                    ResponseType = OpenIdConnectResponseType.IdToken,
                // ValidateIssuer set to false to allow personal and work accounts from any organization to sign in to your application
                // To only allow users from a single organizations, set ValidateIssuer to true and 'tenant' setting in web.config to the tenant name
                // To allow users from only a list of specific organizations, set ValidateIssuer to true and use ValidIssuers parameter 
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false
                },
                // OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = OnAuthenticationFailed
                    }
                }
            );
        }

        /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }
    }
}
