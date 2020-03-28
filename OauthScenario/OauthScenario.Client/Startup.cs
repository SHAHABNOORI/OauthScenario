using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OauthScenario.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
                {
                    // We check the cooki to confirm that er are authenticated
                    config.DefaultAuthenticateScheme = "ClientCookie";

                    // When we sign in we will deal out a cookie
                    config.DefaultSignInScheme = "ClientCookie";

                    // Use this to check if we are allowed to do something
                    config.DefaultChallengeScheme = "OurServer";
                })
                .AddCookie("ClientCookie")
                .AddOAuth("OurServer", config =>
                {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    config.AuthorizationEndpoint = "http://localhost:8860/oauth/authorize";
                    config.TokenEndpoint = "http://localhost:8860/oauth/token";
                    config.CallbackPath = "/oauth/callback";
                });
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();

            //who are you?
            app.UseAuthentication();

            // are you allowed?
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
