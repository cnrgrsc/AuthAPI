using Auth.Core.Config;
using Auth.Core.Model;
using Auth.Core.Repositories;
using Auth.Core.Services;
using Auth.Core.UnitOfWork;
using Auth.Data;
using Auth.Data.Repositories;
using Auth.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAPI
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
            //bu iþlemi normlade autommapper gibi kütüphaneler ile service katmaný içeriisnde yapýlýr
            //DI REgister
            services.AddScoped<IAuthenticationService, AuthenticationService>(); //addscoped ile bir istekle bir örnek oluþsun
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //generic tip olduðu için typeof kullanýldý
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>)); //virgül koyma sebebi generic tip oalrak bir tane almasýndan dolayý

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"), sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("Auth.Data");
                });
            });

            services.AddIdentity<UserApp, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true; //email uniq olsun
                opt.Password.RequireNonAlphanumeric = false; // * ? gibi karekterler zorunlu olmasýn
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //biz þifre sýfýrlanýnca token üreitiyoruz ve daha sonra var olan tokeni öldürüyoruz => þifre sýfýrlama iþlemlerinde tokeni öldürüyoruz (AddDefualtToken)

            services.Configure<CustomTokenOptions>(Configuration.GetSection("TokenOption")); //burada appsteiings içerisinde tokenoptionu okumasýný gerektiðini belirtiyorum
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));

            services.AddAuthentication(options =>
            {
                //kullanýcý giriþince eðer çoklü rol giriþ ivar ise burada bir þema beliritiyoruz
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //AddAuthentication ile AddJwtBearer iki þemayý birbiri ile konuþturuyouruz bunu defaulyolarak kontrol ediyoruz
                //bir istek yapýldýðýnda controller üzerinde tokeni arýyacaðýz
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOptions>(); //burada yukarýda tokenoptiona gitmesini belirttim ve bunun instancený almam gerekiyordu ve instance iþlemini gerçekleþtirdim
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    //burada gelen istekleri kontrol ediyoruz defaul oalrak biz neyi kontrol etmek istersek onu kontrol ediyoruz.
                    ValidIssuer = tokenOptions.Issuer, //Issue kontrol ediyorum
                    ValidAudience = tokenOptions.Audience[0], //
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true, //imzayý doðrula
                    ValidateAudience = true, //audince doðrula
                    ValidateIssuer = true, //issu doðrula 
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero //clockskew 


                };
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
