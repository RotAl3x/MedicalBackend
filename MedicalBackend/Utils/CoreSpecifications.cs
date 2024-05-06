using MedicalBackend.Repositories;
using MedicalBackend.Repositories.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalBackend.Utils;

public static class CoreSpecifications
{
    public static IServiceCollection AddCoreSpecifications(this IServiceCollection services)
    {
        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped(typeof(IBaseRepository<>),typeof(BaseRepository<>));
        services.AddScoped<IAppointmentRepository,AppointmentRepository>();
        services.AddScoped<ISendSmsQueueRepository,SendSmsQueueRepository>();
        return services;
    }
}