using Sphera.API.Billing.BilletConfigurations;
using Sphera.API.Billing.Billets;
using Sphera.API.Billing.BillingEntries;
using Sphera.API.Billing.ClientServicePrices;
using Sphera.API.Billing.Invoices;
using Sphera.API.Billing.Remittances;

namespace Sphera.API.Billing;

public static class BillingModule
{
    public static IServiceCollection ConfigureBillingRelatedDependencies(this IServiceCollection services)
    {
        services
            .ConfigureBillingEntriesRelatedDependencies()
            .ConfigureClientServicePricesRelatedDependencies()
            .ConfigureInvoicesRelatedDependencies()
            .ConfigureBilletsRelatedDependencies()
            .ConfigureBilletConfigurationsRelatedDependencies()
            .ConfigureRemittancesRelatedDependencies();

        return services;
    }
}