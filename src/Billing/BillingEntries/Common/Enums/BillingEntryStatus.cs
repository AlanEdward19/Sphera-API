namespace Sphera.API.Billing.BillingEntries.Common.Enums;

public enum EBillingEntryStatus
{
    Pending = 0,     // Ainda não lançado em faturamento
    Invoiced = 1,    // Já lançado em uma fatura
    Canceled = 2     // Cancelado / desconsiderado
}