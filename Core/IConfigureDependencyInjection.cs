using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    public interface IConfigureDependencyInjection
    {
        int Order { get; }
        void Register(IServiceCollection services, string environmentName);
    }
}