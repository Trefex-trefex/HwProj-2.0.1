using System.Net.Http;
using HwProj.AuthService.Client;
using HwProj.CoursesService.Client;
using HwProj.EventBus.Client.Interfaces;
using HwProj.SolutionsService.API.Models;
using HwProj.SolutionsService.API.Repositories;
using HwProj.SolutionsService.API.Services;
using HwProj.Utils.Configuration;
using HwProj.Utils.FeaturesForSwagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HwProj.SolutionsService.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<SolutionContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<ISolutionsRepository, SolutionsRepository>();
            services.AddScoped<ISolutionsService, Services.SolutionsService>();

            var httpClient = new HttpClient();
            services.AddAuthServiceClient(httpClient, "http://localhost:5001");
            services.AddCoursesServiceClient(httpClient, "http://localhost:5002");

            services.AddEventBus(Configuration);
            services.ConfigureHwProjServices("Solutions API");
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<FormFileOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IEventBus eventBus)
        {
            app.ConfigureHwProj(env, "Solutions API");
        }
    }
}
