using System;
using System.Text;
using System.Threading.Tasks;
using Core.Engine;
using Core.Finder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Model;
using Web.Framework.Engine;
using Web.Framework.Finder;

namespace Web.Framework.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        /// <param name="env">Web host environment</param>
        /// <returns>Configured service provider</returns>
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddCors();
            services.AddControllers().AddNewtonsoftJson();

            // configure strongly typed settings objects
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            ICoreFileProvider coreFileProvider = new WebFileProvider(env);
            services.AddSingleton(coreFileProvider);

            IEngine engine = new WebEngine();
            services.AddSingleton(engine);

            EngineContext.Replace(engine);
            EngineContext.Current.Initialize(services, coreFileProvider, env.EnvironmentName);
        }
    }
}