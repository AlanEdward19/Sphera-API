using Sphera.API.Contacts.AddContactToClient;
using Sphera.API.Contacts.AddContactToPartner;
using Sphera.API.Contacts.AddContactToUser;
using Sphera.API.Contacts.EditContact;
using Sphera.API.Contacts.RemoveContactFromClient;
using Sphera.API.Contacts.RemoveContactFromPartner;
using Sphera.API.Contacts.RemoveContactFromUser;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Contacts;

/// <summary>
/// Provides extension methods for registering contacts-related services and handlers with a dependency injection
/// container.
/// </summary>
public static class ContactsModule
{
    /// <summary>
    /// Adds contacts-related service dependencies to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to which the contacts-related dependencies will be added. Cannot be null.</param>
    /// <returns>The same instance of <see cref="IServiceCollection"/> with contacts-related services registered.</returns>
    public static IServiceCollection ConfigureContactsRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<AddContactToPartnerCommand, ContactDTO>, AddContactToPartnerCommandHandler>();
        services.AddScoped<IHandler<AddContactToClientCommand, ContactDTO>, AddContactToClientCommandHandler>();
        services.AddScoped<IHandler<AddContactToUserCommand, ContactDTO>, AddContactToUserCommandHandler>();
        services.AddScoped<IHandler<EditContactCommand, ContactDTO>, EditContactCommandHandler>();
        services.AddScoped<IHandler<RemoveContactFromPartnerCommand, bool>, RemoveContactFromPartnerCommandHandler>();
        services.AddScoped<IHandler<RemoveContactFromClientCommand, bool>, RemoveContactFromClientCommandHandler>();
        services.AddScoped<IHandler<RemoveContactFromUserCommand, bool>, RemoveContactFromUserCommandHandler>();

        return services;
    }
}