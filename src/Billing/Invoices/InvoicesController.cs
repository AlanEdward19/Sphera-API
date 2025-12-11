using Microsoft.AspNetCore.Mvc;
using Sphera.API.Billing.Invoices.AddInvoiceAdditionalValue;
using Sphera.API.Billing.Invoices.CloseInvoicesForPeriod;
using Sphera.API.Billing.Invoices.DTOs;
using Sphera.API.Billing.Invoices.Enums;
using Sphera.API.Billing.Invoices.GetInvoiceById;
using Sphera.API.Billing.Invoices.ListInvoices;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.Invoices;

[ApiController]
[Route("api/billing/[controller]")]
public class InvoicesController : ControllerBase
{
    /// <summary>
    /// Closes invoices for the specified period and returns the result of the operation.
    /// </summary>
    /// <remarks>This endpoint is typically used to finalize invoices for a given period. The response will
    /// include the closed invoices on success, or an error code and details if the operation fails.</remarks>
    /// <param name="handler">The handler used to process the close invoices command and return the collection of closed invoices.</param>
    /// <param name="command">The command containing the details of the period for which invoices should be closed.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the collection of closed invoices if the operation succeeds;
    /// otherwise, an error response with the failure details.</returns>
    [HttpPut("close")]
    public async Task<IActionResult> CloseInvoices(
        [FromServices] IHandler<CloseInvoicesForPeriodCommand, IReadOnlyCollection<InvoiceDTO>> handler,
        [FromBody] CloseInvoicesForPeriodCommand command,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Ok(response.Success)
            : StatusCode(response.Failure.Code, response.Failure);
    }

    /// <summary>
    /// Retrieves the invoice with the specified unique identifier.
    /// </summary>
    /// <remarks>Returns a 200 OK response with the invoice data if the operation succeeds. If the invoice is
    /// not found or an error occurs, returns an appropriate error response with the corresponding status
    /// code.</remarks>
    /// <param name="handler">The handler used to process the query for retrieving the invoice. Must not be null.</param>
    /// <param name="id">The unique identifier of the invoice to retrieve.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the invoice data if found; otherwise, an error response indicating the
    /// failure reason.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromServices] IHandler<GetInvoiceByIdQuery, InvoiceDTO> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new GetInvoiceByIdQuery(id), HttpContext, cancellationToken);

        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure.Code, response.Failure);
    }

    /// <summary>
    /// Handles an HTTP GET request to retrieve a collection of invoices based on the specified query parameters.
    /// </summary>
    /// <remarks>The response will contain a collection of invoices matching the query criteria, or an error
    /// result if the query fails. This endpoint supports cancellation via the provided token.</remarks>
    /// <param name="handler">The handler responsible for processing the invoice query and returning the result. Must not be null.</param>
    /// <param name="query">The query parameters used to filter and paginate the list of invoices. Cannot be null.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the list of invoices if the operation succeeds; otherwise, an error
    /// response with the appropriate status code.</returns>
    [HttpGet]
    public async Task<IActionResult> List(
        [FromServices] IHandler<ListInvoicesQuery, IReadOnlyCollection<InvoiceDTO>> handler,
        [FromQuery] ListInvoicesQuery query,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);

        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure.Code, response.Failure);
    }

    /// <summary>
    /// Adds an additional value to the specified invoice.
    /// </summary>
    /// <param name="handler">The handler responsible for processing the command to add an additional value to the invoice.</param>
    /// <param name="id">The unique identifier of the invoice to which the additional value will be added.</param>
    /// <param name="command">The command containing the details of the additional value to add to the invoice.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> that represents the result of the operation. Returns a 201 Created response with
    /// the updated invoice if successful; otherwise, returns an error response with the appropriate status code.</returns>
    [HttpPut("{id:guid}/additional-values")]
    public async Task<IActionResult> AddAdditionalValue(
        [FromServices] IHandler<AddInvoiceAdditionalValueCommand, InvoiceDTO> handler,
        Guid id,
        [FromBody] AddInvoiceAdditionalValueCommand command,
        CancellationToken cancellationToken)
    {
        command.SetInvoiceId(id);

        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Created($"/api/billing/invoices/{response.Success.Id}", response.Success)
            : StatusCode(response.Failure.Code, response.Failure);
    }
}