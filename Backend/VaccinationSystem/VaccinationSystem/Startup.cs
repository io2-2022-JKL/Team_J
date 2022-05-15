using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using VaccinationSystem.Config;
using VaccinationSystem.Models;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using System;

namespace VaccinationSystem
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
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                }).ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressMapClientErrors = true;
                });
            //services.AddControllersWithViews();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:6001";

                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidIssuers = new List<string>() { "https://localhost:6001" },
                        ClockSkew = TimeSpan.Zero
                    };
                });
            

            /*
            services.AddAuthentication("token")
                .AddOAuth2Introspection("token", options =>
                {
                    options.Authority = "https://localhost:6001";

                    options.ClientId = "vaccination-system-api";
                    options.ClientSecret = "secret";
                });
            */

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "vaccination-system-api");
                    policy.RequireRole(Role.Admin);
                });
                options.AddPolicy("DoctorPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "vaccination-system-api");
                    policy.RequireRole(Role.Doctor);
                });
                options.AddPolicy("PatientPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "vaccination-system-api");
                    policy.RequireRole(Role.Patient, Role.Doctor);
                });
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("3.0.0",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "VaccinationSystem",
                        Version = "3.0.0"
                    });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "VaccinationSystem.xml");
                options.IncludeXmlComments(filePath);

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme"
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            In = ParameterLocation.Header,
                            Name = "Bearer"
                        },
                        new string[]{ }
                    }
                });

            });

            services.AddCors(options => options.AddPolicy("Cors", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            services.AddHttpClient();

            var connectionString = Configuration.GetConnectionString("AppDb");
            services.AddDbContext<VaccinationSystemDbContext>(x => x.UseSqlServer(connectionString));

            services.AddControllersWithViews();


            // In production, the React files will be served from this directory
            /*services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Build();
            app.UseCors(builder => builder
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("3.0.0/swagger.json", "VaccinationSystem 3.0.0");
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("Cors");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            /*app.Build();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseEndpoints(routes =>
            {
                routes.MapRoute("default", "{controller}/{action}");
            });
            //app.MapControllers();
            */
            /*if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });*/
        }
    }
}
