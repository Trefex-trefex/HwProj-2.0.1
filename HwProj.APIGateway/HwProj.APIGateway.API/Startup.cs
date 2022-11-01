﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using HwProj.APIGateway.API.ExportServices;
using HwProj.APIGateway.API.Models;
using HwProj.AuthService.Client;
using HwProj.CoursesService.Client;
using HwProj.NotificationsService.Client;
using HwProj.SolutionsService.Client;
using HwProj.Utils.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using HwProj.Utils.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;


namespace HwProj.APIGateway.API
{
    public class Startup
    {
        private readonly IConfigurationSection _sheetsConfiguration;

        public Startup(IConfiguration configuration)
        {
            _sheetsConfiguration = configuration.GetSection("GoogleSheets");
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.ConfigureHwProjServices("API Gateway");

            const string authenticationProviderKey = "GatewayKey";

            services.AddAuthentication()
                .AddJwtBearer(authenticationProviderKey, x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "AuthService",
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthorizationKey.SecurityKey,
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddSingleton(_ => ConfigureGoogleSheets(_sheetsConfiguration));
            services.AddSingleton<GoogleService>();

            services.AddAuthServiceClient();
            services.AddCoursesServiceClient();
            services.AddSolutionServiceClient();
            services.AddNotificationsServiceClient();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ConfigureHwProj(env, "API Gateway");
        }

        private static JToken Serialize(IConfigurationSection configurationSecton)
        {
            JObject obj = new JObject();
            foreach (var child in configurationSecton.GetChildren())
            {
                obj.Add(child.Key, child.Value);
            }

            return obj;
        }

        private static SheetsService ConfigureGoogleSheets(IConfigurationSection _sheetsConfiguration)
        {
            var jsonObject = Serialize(_sheetsConfiguration);
            var credential = GoogleCredential.FromJson(jsonObject.ToString()).CreateScoped(SheetsService.Scope.Spreadsheets);

            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "HwProjSheets"
            });
        }
    }
}
