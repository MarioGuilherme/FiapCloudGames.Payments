using FiapCloudGames.Payments.Domain.Messaging;
using FiapCloudGames.Payments.Domain.Repositories;
using FiapCloudGames.Payments.Infrastructure.Messaging.RabbitMq;
using FiapCloudGames.Payments.Infrastructure.Persistence;
using FiapCloudGames.Payments.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Payments.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMessageBroker(configuration)
            .AddDbContext(configuration)
            .AddRepositories()
            .AddUnitOfWork();

        return services;
    }

    private static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddSingleton<IEventPublisher, RabbitMqPublisher>();

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string dbHost = configuration["Database:Host"]!;
        string dbName = configuration["Database:Name"]!;
        string dbUser = configuration["DB_USER"]!;
        string dbPassword = configuration["DB_PASSWORD"]!;
        string connectionString = $"Server={dbHost};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=True";

        services.AddDbContext<FiapCloudGamesPaymentsDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
