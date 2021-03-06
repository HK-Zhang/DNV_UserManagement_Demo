using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DNVGL.Authorization.UserManagement.ApiControllers;
using DNVGL.Authorization.UserManagement.EFCore;
using DNVGL.Authorization.Web;
using DNVGL.OAuth.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CompanyRoleUser.AspCore3.EFCore5
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
            services.AddRazorPages();

            services.AddOidc(o =>
            {
                o.ClientId = "1c19b0b5-506d-4d41-b31c-3d3fd75c481d";
                o.Scopes = new[] { "offline_access", "https://dnvglb2cprod.onmicrosoft.com/83054ebf-1d7b-43f5-82ad-b2bde84d7b75/user_impersonation" };
                o.CallbackPath = "/signin-oidc";
                o.Authority = "https://login.veracity.com/dnvglb2cprod.onmicrosoft.com/B2C_1A_SignInWithADFSIdp/v2.0";
            }, cookieOption => cookieOption.Events.AddCookieValidateHandler());

            services.AddUserManagement().UseEFCore(new EFCoreOptions
            {
                DbContextOptionsBuilder = options => options.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=UserManagement;Trusted_Connection=Yes;")
            });


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                // swagger documentaion group for User Management.
                c.SwaggerDoc("UserManagement", new OpenApiInfo
                {
                    Title = "User Management",
                    Version = "v1"
                });

                // swagger documentaion group for your system.
                c.SwaggerDoc("WebAPI", new OpenApiInfo
                {
                    Title = "Web API",
                    Version = "v1"
                });

                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                c.DocInclusionPredicate((name, api) =>
                {
                    if (name == "UserManagement")
                        return api.GroupName != null && api.GroupName.StartsWith("UserManagement");
                    else
                        return api.GroupName == null;
                });

                var xmlFile = $"DNVGL.Authorization.UserManagement.ApiControllers.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "apidocs", xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            //services.AddUserManagement().UseEFCore
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.Use(async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    await context.ChallengeAsync();
                }
                else
                {
                    await next();
                }
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/UserManagement/swagger.json", "User Management API v1");
                c.SwaggerEndpoint("/swagger/WebAPI/swagger.json", "Web API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
            });

        }
    }
}
