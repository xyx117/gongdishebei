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

            //ҵ��������
            services.AddDbContext<GongDiContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("GongDiContext")));

            //���� ApplicationDbContext�� Identity ������
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("GongDiContext"),b=>b.MigrationsAssembly("GongDiJiXie")));

            //ע�� ApplicationDbContext ��identity�����Ĺ��� IdentityUser
            //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //        .AddEntityFrameworkStores<ApplicationDbContext>();            

            //������������������
            services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
                options.SignIn.RequireConfirmedAccount = false;//�����Ϊ true �󣬵�¼�쳣�����⣻���﹦�������ڣ�ע���û�����Ҫ �����ֶ�ȷ�� �û���ʵ�ԣ����� ����ȷ�ϣ��绰����ȷ�ϵ�
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

            //������������������
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

                options.LoginPath = "/Account/Login";  //�ж��Ƿ��¼����ת����¼ҳ��
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            //������������  ����Ч��
            //services.AddControllers().AddJsonOptions(cfg =>
            //{
            //    cfg.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            //});

            //������������  ��Ч��   �ο�     www.cnblogs.com/dudu/p/5879913.html
            //services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            //������������  ��Ч��     www.cnblogs.com/dudu/p/5879913.html
            //services.Configure<WebEncoderOptions>(options =>
            //{
            //    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            //});

            //������������  ��Ч��   www.cnblogs.com/dudu/p/5879913.html
            services.Configure<WebEncoderOptions>(options =>options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin,
                                                        UnicodeRanges.CjkUnifiedIdeographs));

            services.AddAuthorization(option =>
            {
                option.AddPolicy("���޹���Ա", policy => policy.RequireRole("admin"));
                option.AddPolicy("�༭�û�", policy => policy.RequireClaim("Edit","Edit"));  //Claim��һ�� ��ֵ��
                option.AddPolicy("ֻ��admin", policy => policy.RequireUserName("admin"));  //ֻ������û����Է���

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

            app.UseAuthentication();  //��¼��֤������Ҫ����  app.UseAuthorization() ǰ�棬©����������2��
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
