// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer.Models;
using IdentityServer.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using IdentityServer4.Services;
using IdentityServer.Services;
using IdentityServer4.KeyManagement.EntityFramework;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            // services.AddControllersWithViews();

            services.AddControllers();

            var connectionString = Configuration.GetConnectionString("IdentityServerDatabase");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            /*services.AddKeyManagementDbContext(new DatabaseKeyManagementOptions
            {
                ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly))
            });*/

            services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddDeveloperSigningCredential();
                /*.AddSigningKeyManagement(options =>
                {
                    options.License = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjItMDYtMTNUMDE6MDA6MDEuOTE0MTUwMyswMDowMCIsImlhdCI6IjIwMjItMDUtMTRUMDE6MDA6MDEiLCJvcmciOiJERU1PIiwiYXVkIjo1fQ==.JhTaaGrYXi9P7k5CPyGZAPPjxbk7ZJkbAfvXoIV8hmN4O+RelCHJ8PJNFuoqE9G2NWYZLiPT9wXphLYKlAIB4nwrjEUl4f1DNJ5Qhrta1GebcuKQOec2uhNOOejyn7EVzZ0T7Xg9RNuF0/UCoQbDGPsjEijkSmiVr6TNLBuwZYV1Yi6e8SqtsTW2CaCqBjjSoR8fXyWQm/wi7CsI0kumFIn2pR0lFS/dTZBrvXwsMUnqYkGHZF2TuyElPejxMdnOEgxD4/E6rDwTPzR/Y4m1TbquqwCzGHn9Do5jRkALBKLEhkYzvEo8oE5/0ap5+uRQXGM6hiiHsjjM6DHW6cVi8Pb+PWteezS5HuiQUnfS+5uVtde1WGM+YKfvfCAQQL5waMht4Aenxr3o19uwYtsgFY6rymINE6ajimxxwRdbnfOYNU96NACey9EWXcv3eq6C6nVIZJBm1iJc7Zw0zdB3Puh5bhFbtf6TXDUevPyNMY9cx4dyys1eH2tZHZl2QDiEQURSDXEJR5w/FSV1PmN1TLHTrkyz0zsi+cS58o2Csf8moS5OmqPGm7bMarODKkHd0HgFdlxGrOhBT4MjvOqyMUgQy4o6veN+5Jx5vf3PvsQcorWVLImnMH7++61hSuUgsMz3r/82phD4GyhKfJQ6TRsEAlBMqkEeUhFDCZ5ctYg=";
                    options.Licensee = "DEMO";
                })
                .PersistKeysToDatabase(new DatabaseKeyManagementOptions()
                {
                    ConfigureDbContext = b => b.UseSqlServer(connectionString)
                })
                .EnableInMemoryCaching();*/
            //.ProtectKeysWithDataProtection();

            services.AddScoped<IProfileService, ProfileService>();

            services.AddHttpClient();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //context.Database.Migrate();

            DBInitializer.PopulateIdentityServer(app);

            UserInitializer.Initialize(app);

            app.Build();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseAuthorization();
            /*app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });*/

            app.UseIdentityServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
