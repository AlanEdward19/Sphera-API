using Sphera.API.Documents.CreateDocument;
using Sphera.API.Documents.DeleteDocument;
using Sphera.API.Documents.DTOs;
using Sphera.API.Documents.GetDocumentById;
using Sphera.API.Documents.GetDocuments;
using Sphera.API.Documents.UpdateDocument;
using Sphera.API.Documents.UploadDocument;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Documents;

public static class DocumentsModule
{
    public static IServiceCollection ConfigureDocumentsRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }

    /// <summary>
    /// Registers command and query handler services for client-related operations in the dependency injection
    /// container.
    /// </summary>
    /// <remarks>This extension method adds scoped implementations for handlers managing client creation,
    /// update, deletion, and retrieval. It enables the application to resolve these handlers via dependency injection
    /// for processing client commands and queries.</remarks>
    /// <param name="services">The service collection to which the handler services will be added.</param>
    /// <returns>The same service collection instance with the handler services registered.</returns>
    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateDocumentCommand, DocumentDTO>, CreateDocumentCommandHandler>();
        services.AddScoped<IHandler<UploadDocumentCommand, bool>, UploadDocumentCommandHandler>();
        services.AddScoped<IHandler<DeleteDocumentCommand, bool>, DeleteDocumentCommandHandler>();
        services.AddScoped<IHandler<UpdateDocumentCommand, DocumentDTO>, UpdateDocumentCommandHandler>();
        services.AddScoped<IHandler<GetDocumentsQuery, IEnumerable<DocumentWithMetadataDTO>>, GetDocumentsQueryHandler>();
        services.AddScoped<IHandler<GetDocumentByIdQuery, DocumentWithMetadataDTO>, GetDocumentByIdQueryHandler>();

        return services;
    }
}