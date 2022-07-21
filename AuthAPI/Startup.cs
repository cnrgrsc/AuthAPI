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
            //bu i�lemi normlade autommapper gibi k�t�phaneler ile service katman� i�eriisnde yap�l�r
            //DI REgister
            services.AddScoped<IAuthenticationService, AuthenticationService>(); //addscoped ile bir istekle bir �rnek olu�sun
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //generic tip oldu�u i�in typeof kullan�ld�
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>)); //virg�l koyma sebebi generic tip oalrak bir tane almas�ndan dolay�

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
                opt.Password.RequireNonAlphanumeric = false; // * ? gibi karekterler zorunlu olmas�n
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //biz �ifre s�f�rlan�nca token �reitiyoruz ve daha sonra var olan tokeni �ld�r�yoruz => �ifre s�f�rlama i�lemlerinde tokeni �ld�r�yoruz (AddDefualtToken)

            services.Configure<CustomTokenOptions>(Configuration.GetSection("TokenOption")); //burada appsteiings i�erisinde tokenoptionu okumas�n� gerekti�ini belirtiyorum
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));

            services.AddAuthentication(options =>
            {
                //kullan�c� giri�ince e�er �okl� rol giri� ivar ise burada bir �ema beliritiyoruz
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //AddAuthentication ile AddJwtBearer iki �emay� birbiri ile konu�turuyouruz bunu defaulyolarak kontrol ediyoruz
                //bir istek yap�ld���nda controller �zerinde tokeni ar�yaca��z
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOptions>(); //burada yukar�da tokenoptiona gitmesini belirttim ve bunun instancen� almam gerekiyordu ve instance i�lemini ger�ekle�tirdim
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    //burada gelen istekleri kontrol ediyoruz defaul oalrak biz neyi kontrol etmek istersek onu kontrol ediyoruz.
                    ValidIssuer = tokenOptions.Issuer, //Issue kontrol ediyorum
                    ValidAudience = tokenOptions.Audience[0], //
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true, //imzay� do�rula
                    ValidateAudience = true, //audince do�rula
                    ValidateIssuer = true, //issu do�rula 
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
