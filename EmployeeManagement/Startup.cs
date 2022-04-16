using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<EmDbContext>(o => o.UseSqlServer(_config.GetConnectionString("EmDbConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequiredLength = 4;  //configure password options
                o.Password.RequiredUniqueChars = 3;
                o.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<EmDbContext>();

            ///// does the same thing as above
            //services.Configure<IdentityOptions>(o =>
            //{
            //    o.Password.RequiredLength = 10;
            //    o.Password.RequiredUniqueChars = 3;
            //});

            services.AddMvc(options =>
            {
                options.Filters.Add(    //enable global authorization
                    new AuthorizeFilter(
                        new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
                options.EnableEndpointRouting = false;
            });
            services.AddAuthorization(o =>
            {
                //claim policy
                o.AddPolicy("CreateRolePolicy", p => p.RequireClaim("Create Role", "true"));
                //o.AddPolicy("EditRolePolicy", p => p.RequireRole("Admin").RequireClaim("Edit Role", "true").RequireRole("Super Admin"));
                o.AddPolicy("EditRolePolicy", p => p.RequireAssertion(EditRoleHandler));  //custom policy via assertion
                o.AddPolicy("EditUserRolesAndClaimsPolicy", p => p.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));   //custom policy without Func that requires logged in user and other services via DI
                o.AddPolicy("DeleteRolePolicy", p => p.RequireClaim("Delete Role", "true"));    //chain RequireClaim(...) for AND
                //role policy
                //o.AddPolicy("AdminRolePolicy", p => p.RequireRole("Admin"));    //using claim instead of role
            });
            //services.AddMvcCore(options => options.EnableEndpointRouting = false).AddXmlSerializerFormatters();
            services.ConfigureApplicationCookie(o =>
            {
                o.LoginPath = new PathString("/Account/Login"); //configure login path (default: /Account/Login)
                o.AccessDeniedPath = new PathString("/Account/AccessDenied");  //configure accessdenied path (default: /Account/AccessDenied)
            });
            //var serviceDescriptor = new ServiceDescriptor(typeof(IEmployeeRepository), typeof(MockEmployeeRepository), ServiceLifetime.Singleton);
            services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();

            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();  //handlers for same requirement, performs OR logic
        }

        private static bool EditRoleHandler(AuthorizationHandlerContext context)
        {
            return (context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "Edit Role" && c.Value == "true")) || context.User.IsInRole("Super Admin");
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
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();
            //app.UseMvcWithDefaultRoute();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc(r =>
            {
                r.MapRoute("default", "{controller=home}/{action=index}/{id?}");
            });   //conventional routing

            //app.UseMvc();

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync($"Hello world!");

            //});
            //app.UseEndpoints(builder =>
            //{
            //    builder.MapDefaultControllerRoute();
            //});
        }

        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        //var options = new DeveloperExceptionPageOptions()
        //        //{
        //        //    SourceCodeLineCount = 3
        //        //};
        //        //app.UseExceptionHandler("/Error");
        //        app.UseDeveloperExceptionPage();
        //    }


        //    app.UseRouting();

        //    //app.UseEndpoints(endpoints =>
        //    //{
        //    //    endpoints.MapGet("/", async context =>
        //    //    {
        //    //        //await context.Response.WriteAsync($"Process: {System.Diagnostics.Process.GetCurrentProcess().ProcessName}, MyKey: {_config["MyKey"]}, ASPNETCORE_ENVIRONMENT : {_config["ASPNETCORE_ENVIRONMENT"]}");
        //    //        await context.Response.WriteAsync($"This is from first middleware!");
        //    //    });
        //    //});

        //    //app.Use(async (context, next) =>
        //    //{
        //    //    //await context.Response.WriteAsync($"This is from first middleware!");
        //    //    logger.LogInformation("MW1: Incoming request.");
        //    //    await next();
        //    //    logger.LogInformation("MW1: Outgoing response.");
        //    //});

        //    //app.Use(async (context, next) =>
        //    //{
        //    //    //await context.Response.WriteAsync($"This is from second middleware!");
        //    //    logger.LogInformation("MW2: Incoming request.");
        //    //    await next();
        //    //    logger.LogInformation("MW2: Outgoing response.");
        //    //});

        //    //var options = new DefaultFilesOptions();
        //    //options.DefaultFileNames.Clear();
        //    //options.DefaultFileNames.Add("foo.html");

        //    //app.UseDefaultFiles(options);
        //    app.UseStaticFiles();

        //    //var options = new FileServerOptions
        //    //{
        //    //    EnableDirectoryBrowsing = true
        //    //};
        //    //options.DefaultFilesOptions.DefaultFileNames.Clear();
        //    //options.DefaultFilesOptions.DefaultFileNames.Add("foo.html");

        //    //app.UseFileServer();

        //    //app.UseDirectoryBrowser();

        //    app.Run(async context =>
        //    {
        //        await context.Response.WriteAsync($"Hello world!");
        //        //await context.Response.WriteAsync($"This is from terminal middleware!");
        //        //logger.LogInformation("MWT: Incoming request.");
        //        //throw new Exception("Some error occured!");
        //        //await context.Response.WriteAsync($"Hosting Environment: {env.EnvironmentName}");
        //        //logger.LogInformation("MWT: Outgoing response.");
        //    });
        //}
    }
}
