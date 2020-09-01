using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazorApp1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.Authority = "https://dev-12345.okta.com/oauth2/default"; //[changed]your okta authority here
                options.ProviderOptions.ClientId = "0oaf9vjq1qqI3JhJL4x6"; //[changed]your oktaclient Id here
                options.ProviderOptions.ResponseType = "id_token token";
            });

            //This will attach access tokens (Oauth2) for every backend request (api.someWebsite.co.uk)
            builder.Services.AddHttpClient("AuthenticatedClient",
                    client => client.BaseAddress = new Uri("http://api.someWebsite.co.uk"))
                .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler(
                        new[] { "http://api.someWebsite.co.uk" },
                        scopes: new[] { "openId" }));

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("AuthenticatedClient"));

            await builder.Build().RunAsync();
        }
    }
}
