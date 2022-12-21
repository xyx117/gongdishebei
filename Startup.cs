using GongDiJiXie.Data;
using GongDiJiXie.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace GongDiJiXie
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
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddRazorPages();           
            services.AddMvc();            

            //业务上下文
            services.AddDbContext<GongDiContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("GongDiContext")));

            //配置 ApplicationDbContext， Identity 上下文
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("GongDiContext"),b=>b.MigrationsAssembly("GongDiJiXie")));

            //注册 ApplicationDbContext ，identity上下文关联 IdentityUser
            //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //        .AddEntityFrameworkStores<ApplicationDbContext>();            

            //这里两个区别在哪里
            services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
                options.SignIn.RequireConfirmedAccount = false;//这里改为 true 后，登录异常，不解；这里功能类似于，注册用户后需要 另外手段确认 用户真实性，例如 邮箱确认，电话短信确认等
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddRoles<ApplicationRole>().AddDefaultTokenProviders().AddDefaultUI();

            //这里两个区别在哪里
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Account/Login";  //判断是否登录，跳转到登录页面
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            //处理中文乱码  不起效果
            //services.AddControllers().AddJsonOptions(cfg =>
            //{
            //    cfg.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            //});

            //处理中文乱码  起效果   参考     www.cnblogs.com/dudu/p/5879913.html
            //services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            //处理中文乱码  起效果     www.cnblogs.com/dudu/p/5879913.html
            //services.Configure<WebEncoderOptions>(options =>
            //{
            //    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            //});

            //处理中文乱码  起效果   www.cnblogs.com/dudu/p/5879913.html
            services.Configure<WebEncoderOptions>(options =>options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin,
                                                        UnicodeRanges.CjkUnifiedIdeographs));

            services.AddAuthorization(option =>
            {
                option.AddPolicy("仅限管理员", policy => policy.RequireRole("admin"));
                option.AddPolicy("编辑用户", policy => policy.RequireClaim("Edit","Edit"));  //Claim是一个 键值对
                option.AddPolicy("只有admin", policy => policy.RequireUserName("admin"));  //只有这个用户可以访问

            });

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

            app.UseRouting();

            app.UseAuthentication();  //登录验证，这里要放在  app.UseAuthorization() 前面，漏这句代码困了2天
            app.UseAuthorization();

            //app.MapRazorPages();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
        }
    }
}
