using Sphera.API.Billing.Invoices.AddInvoiceAdditionalValue;
using Sphera.API.Billing.Invoices.CloseInvoicesForPeriod;
using Sphera.API.Billing.Invoices.DTOs;
using Sphera.API.Billing.Invoices.CreateInvoice;
using Sphera.API.Billing.Invoices.GetInvoiceById;
using Sphera.API.Billing.Invoices.ListInvoices;
using Sphera.API.Billing.Invoices.EditInvoiceItem;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Invoices;

public static class InvoicesModule
{
    public static IServiceCollection ConfigureInvoicesRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureHandlers();

        return services;
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<CloseInvoicesForPeriodCommand, IReadOnlyCollection<InvoiceDTO>>, CloseInvoicesForPeriodCommandHandler>();
        services.AddScoped<IHandler<GetInvoiceByIdQuery, InvoiceDTO>, GetInvoiceByIdQueryHandler>();
        services.AddScoped<IHandler<ListInvoicesQuery, IReadOnlyCollection<InvoiceDTO>>, ListInvoicesQueryHandler>();
        services.AddScoped<IHandler<AddInvoiceAdditionalValueCommand, InvoiceDTO>, AddInvoiceAdditionalValueCommandHandler>();
        services.AddScoped<IHandler<CreateInvoiceCommand, InvoiceDTO>, CreateInvoiceCommandHandler>();
        services.AddScoped<IHandler<EditInvoiceItemCommand, InvoiceDTO>, EditInvoiceItemCommandHandler>();
        
        return services;
    }
}