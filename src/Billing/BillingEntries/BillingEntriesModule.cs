using Sphera.API.Billing.BillingEntries.CreateBillingEntry;
using Sphera.API.Billing.BillingEntries.GetBillingEntryById;
using Sphera.API.Billing.BillingEntries.ListBillingEntries;
using Sphera.API.Billing.BillingEntries.UpdateBillingEntry;
using Sphera.API.Billing.BillingEntries.MarkAsInvoicedBatch;
using Sphera.API.Billing.BillingEntries.CancelBatch;
using Sphera.API.Billing.BillingEntries.ReopenBatch;
using Sphera.API.Billing.BillingEntries.Common;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.BillingEntries;

public static class BillingEntriesModule
{
    public static IServiceCollection ConfigureBillingEntriesRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateBillingEntryCommand, BillingEntryDTO>, CreateBillingEntryCommandHandler>();
        services.AddScoped<IHandler<GetBillingEntryByIdQuery, BillingEntryDTO>, GetBillingEntryByIdQueryHandler>();
        services.AddScoped<IHandler<ListBillingEntriesQuery, IReadOnlyCollection<BillingEntryDTO>>, ListBillingEntriesQueryHandler>();
        services.AddScoped<IHandler<UpdateBillingEntryCommand, BillingEntryDTO>, UpdateBillingEntryCommandHandler>();
        // Batch handlers
        services.AddScoped<IHandler<MarkAsInvoicedBatchCommand, BulkActionResultDTO>, MarkAsInvoicedBatchCommandHandler>();
        services.AddScoped<IHandler<CancelBillingEntriesCommand, BulkActionResultDTO>, CancelBillingEntriesCommandHandler>();
        services.AddScoped<IHandler<ReopenBillingEntriesCommand, BulkActionResultDTO>, ReopenBillingEntriesCommandHandler>();

        return services;
    }
}