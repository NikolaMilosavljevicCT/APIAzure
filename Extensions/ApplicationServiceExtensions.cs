using API.DataManagement.Extensions.Events;
using API.DataManagement.Interfaces;
using API.DataManagement.Interfaces.EventsInterface;
using API.DataManagement.Interfaces.OracleInterface;
using API.DataManagement.Repositories;
using API.DataManagement.Repositories.EventsRepository;
using API.DataManagement.Repositories.Forms;
using API.DataManagement.Repositories.OracleRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.DataManagement.Extensions

{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services, IConfiguration config)
        {
            _ = new EventService();

            services.AddScoped<IMSSQLRepository, MSSQLRepository>();
            services.AddScoped<IFormDefaultDisplayRepository, FormDefaultDisplayRepository>();
            services.AddScoped<IOracleRepository, OracleRepository>();
            services.AddScoped<IEventsRepository, EventsRepository>();

            return services;
        }
    }
}