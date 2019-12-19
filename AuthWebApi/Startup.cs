using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace AuthWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            DependencyInjection(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3(settings =>
            {
                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = Configuration.GetValue<string>("NswagSettings:UseSwaggerUi3:ClientId"),
                    AppName = Configuration.GetValue<string>("NswagSettings:UseSwaggerUi3:AppName")
                };
            });
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void DependencyInjection(IServiceCollection services)
        {
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "ToDo API";
                    document.Info.Description = "A simple ASP.NET Core web API";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "",
                        Email = string.Empty,
                        Url = ""
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = "",
                        Url = ""
                    };
                };
                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
                config.AddSecurity(
                    name: "bearer",
                    globalScopeNames: Enumerable.Empty<string>(),
                    swaggerSecurityScheme: new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Description = "B2C authentication",
                        Flow = OpenApiOAuth2Flow.Implicit,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow()
                            {
                                AuthorizationUrl = Configuration.GetValue<string>("NswagSettings:Security:AuthorizationUrl"),
                                TokenUrl = Configuration.GetValue<string>("NswagSettings:Security:TokenUrl"),
                                Scopes = new Dictionary<string, string>
                                {
                                    { Configuration.GetValue<string>("NswagSettings:Security:ScopePath"), Configuration.GetValue<string>("NswagSettings:Security:ScopeDescription") },
                                }
                            }
                        }
                    });
            });

            // Authentication
            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
               .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));
        }
    }
}
