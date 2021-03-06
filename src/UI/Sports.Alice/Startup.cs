﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sports.Alice.Infrastructure;
using Sports.Alice.Models.Settings;
using Sports.Alice.Scenes;
using Sports.Alice.Services;
using Sports.Alice.Services.Interfaces;
using Sports.Alice.Workers;
using Sports.Data.Context;
using Sports.Data.Services;
using Sports.Data.Services.Interfaces;
using Sports.Services;
using Sports.Services.Interfaces;
using Sports.SportsRu.Api;
using Sports.SportsRu.Api.Services;
using Sports.SportsRu.Api.Services.Interfaces;

namespace Sports.Alice
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
            services.AddControllers();

            string connectionString = Configuration.GetConnectionString("database");
            services.AddDbContext<SportsContext>(builder
                => builder.ConfigureSportsContextOptions(connectionString));

            services.AddScoped<INewsArticleDataService, NewsArticleDataService>();

            services.AddScoped<IAliceService, AliceService>();
            services.AddScoped<ISyncService, SyncService>();
            services.AddSingleton<ISportsRuApiService, SportsRuApiService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<INewsArticleCommentService, NewsArticleCommentService>();

            services.AddScoped<IScenesProvider, ScenesProvider>();
            services.AddScoped<WelcomeScene>();
            services.AddScoped<LatestNewsScene>();
            services.AddScoped<MainNewsScene>();
            services.AddScoped<BestCommentsScene>();
            services.AddScoped<HelpScene>();

            var sportsSettingsSection = Configuration.GetSection("SportsSettings");
            SportsSettings sportsSettings = new();
            sportsSettingsSection.Bind(sportsSettings);
            services.AddSingleton(sportsSettings);

            var sportsRuApiSettingsSection = Configuration.GetSection("SportsRuApiSettings");
            var sportsRuApiSettings = new SportsRuApiSettings();
            sportsRuApiSettingsSection.Bind(sportsRuApiSettings);
            services.AddSingleton(sportsRuApiSettings);

            services.AddHostedService<SyncNewsWorker>();
            services.AddHostedService<SyncNewsCommentsWorker>();
            services.AddHostedService<CleanWorker>();

            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
