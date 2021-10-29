using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OrgDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgAPI
{
    public class Startup
    {
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllers(
            //    config => config.Filters.Add(new MyExceptionFilter()));
            //services.AddCors(
            //    x => x.AddPolicy("myPolicy",
            //    p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins("").AllowCredentials()));
            services.AddControllers(x=>x.Filters.Add(new AuthorizeFilter())).AddXmlSerializerFormatters().AddXmlDataContractSerializerFormatters();
            services.AddDbContext<OrganizationDbContext>();
            services.AddSwaggerDocument();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<OrganizationDbContext>()
                .AddDefaultTokenProviders();
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-my-secret-key"));
            var tokenValidationParameter = new TokenValidationParameters()
            {
                IssuerSigningKey = signingKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                //ValidateLifetime=false,
                //ClockSkew=TimeSpan.Zero
            };


            services.AddAuthentication(x => x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(jwt =>
                    {
                        //jwt.SaveToken = true;
                        //jwt.RequireHttpsMetadata = true;
                        jwt.TokenValidationParameters = tokenValidationParameter;
                    }
                    );
            //services.ConfigureApplicationCookie(opt =>
            //{
            //    opt.ExpireTimeSpan = new TimeSpan(0, 30, 30);
            //    opt.Events = new CookieAuthenticationEvents
            //    {

            //        OnRedirectToLogin = redirectContext =>
            //        {
            //            redirectContext.HttpContext.Response.StatusCode = 401;
            //            return Task.CompletedTask;
            //        },
            //        OnRedirectToAccessDenied = redirectContext =>
            //        {
            //            redirectContext.HttpContext.Response.StatusCode = 401;
            //            return Task.CompletedTask;
            //        }
            //    };

            //});
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(
                options =>
                {
                    options.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("We are working on it");
                        //var ex = context.Features.Get<IExceptionHandlerFeature>();
                        //if (ex != null)
                        //{
                        //    await context.Response.WriteAsync(ex.Error.Message);
                        //}
                    });
                });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseDeveloperExceptionPage();
            app.UseOpenApi();
            //app.UseCors("myPolicy");
            app.UseSwaggerUi3();
        }
    }
}
