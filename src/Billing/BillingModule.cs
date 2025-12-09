using Sphera.API.Billing.BillingEntries;
using Sphera.API.Billing.ClientServicePrices;
using Sphera.API.Billing.Invoices;

namespace Sphera.API.Billing;

public static class BillingModule
{
    public static IServiceCollection ConfigureBillingRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureBillingEntriesRelatedDependencies()
            .ConfigureClientServicePricesRelatedDependencies()
            .ConfigureInvoicesRelatedDependencies();

        return services;
    }
}