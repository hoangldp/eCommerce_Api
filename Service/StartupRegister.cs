using Api.Services;
using Core;
using Microsoft.Extensions.DependencyInjection;

namespace Service
{
    public class StartupRegister : IConfigureDependencyInjection
    {
        public int Order { get; }
        public void Register(IServiceCollection services, string environmentName)
        {
            services.AddScoped<IUserService, UserService>();
        }
    }
}