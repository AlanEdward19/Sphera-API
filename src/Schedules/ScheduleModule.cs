using Sphera.API.Schedules.CreateScheduleEvent;
using Sphera.API.Schedules.DeleteScheduleEvent;
using Sphera.API.Schedules.GetScheduleEvents;
using Sphera.API.Schedules.GetUserScheduleEvents;
using Sphera.API.Schedules.UpdateScheduleEvent;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Schedules;

/// <summary>
/// Provides extension methods for registering schedule-related services and handlers with a dependency injection
/// container.
/// </summary>
/// <remarks>This class is intended to be used during application startup to configure all dependencies required
/// for schedule event operations. All methods are static and designed for use with dependency injection frameworks such
/// as ASP.NET Core's built-in service container.</remarks>
public static class ScheduleModule
{
    /// <summary>
    /// Adds schedule-related service dependencies to the specified service collection.
    /// </summary>
    /// <remarks>This method is intended to be called during application startup to register all services
    /// required for schedule-related functionality. It enables fluent configuration of dependency injection.</remarks>
    /// <param name="services">The service collection to which the schedule-related dependencies will be added. Cannot be null.</param>
    /// <returns>The same instance of <see cref="IServiceCollection"/> that was provided, to allow for method chaining.</returns>
    public static IServiceCollection ConfigureSchedulesRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }

    /// <summary>
    /// Registers command and query handler services required for schedule event operations with the dependency
    /// injection container.
    /// </summary>
    /// <param name="services">The service collection to which the handler services will be added. Cannot be null.</param>
    /// <returns>The same service collection instance with the handler services registered.</returns>
    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateScheduleEventCommand, ScheduleEventDTO>, CreateScheduleEventCommandHandler>();
        services.AddScoped<IHandler<UpdateScheduleEventCommand, ScheduleEventDTO>, UpdateScheduleEventCommandHandler>();
        services.AddScoped<IHandler<DeleteScheduleEventCommand, bool>, DeleteScheduleEventCommandHandler>();
        services.AddScoped<IHandler<GetUserScheduleEventsQuery, IEnumerable<ScheduleEventDTO>>, GetUserScheduleEventsQueryHandler>();
        services.AddScoped<IHandler<GetScheduleEventsQuery, IEnumerable<ScheduleEventDTO>>, GetScheduleEventsQueryHandler>();

        return services;
    }
}