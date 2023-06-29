using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models;
using Microsoft.EntityFrameworkCore;
using TFM104MVC.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using TFM104MVC.Database;
using System.Net.Http;
//using TFM104MVC.Helpers;

namespace TFM104MVC
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
            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opt =>
                {
                    opt.LoginPath = "/home/index";
                    //FB登入
                }).AddFacebook(opt =>
                {
                    opt.AppId = "461446905740380";
                    opt.AppSecret = "9521db1cab2a6400f1c62ff393069bae";
                    //Google登入
                }).AddGoogle(opt =>
                {
                    opt.ClientId = "606720770901-44nd2h3g3u9fj377illetmef7nfrefa7.apps.googleusercontent.com";
                    opt.ClientSecret = "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";
                }); ;
            

            //---FB用(修)
            //services.AddAuthentication(opt =>
            //{
            //    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //}).AddCookie(opt =>
            //{
            //    opt.LoginPath = "/Shared/_Layout";
            //}).AddFacebook(opt =>
            //{
            //    opt.AppId = "461446905740380";
            //    opt.AppSecret = "9521db1cab2a6400f1c62ff393069bae";
            //});
            //

            services.AddControllersWithViews();
            services.AddControllers(setupAction => setupAction.ReturnHttpNotAcceptable = true)
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "type",
                            Title = "數據驗證失敗",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "請看詳細內容",
                            Instance = context.HttpContext.Request.Path
                        };
                        problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IAuthenticateRepository, AuthenticateRepository>();
            services.AddSingleton<ISender, EmailSender>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();

            // 將 Session 存在 ASP.NET Core 記憶體中
            services.AddDistributedMemoryCache();
            //註冊session服務
            services.AddSession();

            services.AddDbContext<AppDbContext>(builder => builder.UseSqlServer(Configuration.GetConnectionString("MVC")));
            //掃描profile檔案
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(); //表示 你在哪

            app.UseAuthentication(); // 表示 你是誰

            app.UseAuthorization(); // 表示 你可以幹什麼

            // SessionMiddleware 加入 Pipeline
            app.UseSession();

            //試跑一次看有沒有加到 //結果確實有加到
            //app.Run(async (context) =>
            //{
            //    context.Session.SetString("Sample", "This is Session.");
            //    string message = context.Session.GetString("Sample");
            //    await context.Response.WriteAsync($"{message}");
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
