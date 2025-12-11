using Microsoft.AspNetCore.Mvc;
using Sphera.API.Billing.BillingEntries.CreateBillingEntry;
using Sphera.API.Billing.BillingEntries.GetBillingEntryById;
using Sphera.API.Billing.BillingEntries.ListBillingEntries;
using Sphera.API.Billing.BillingEntries.UpdateBillingEntry;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.BillingEntries;

[ApiController]
[Route("api/v1/billing/[controller]")]
public class BillingEntriesController : ControllerBase
{
    /// <summary>
    /// Creates a new billing entry using the specified command and returns the result of the operation.
    /// </summary>
    /// <remarks>If the creation is successful, the response includes a location header referencing the newly
    /// created billing entry. If the operation fails, the response contains error details and the corresponding status
    /// code.</remarks>
    /// <param name="handler">The handler responsible for processing the <paramref name="command"/> and returning the billing entry data
    /// transfer object.</param>
    /// <param name="command">The command containing the details required to create a new billing entry. Cannot be null.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the creation operation. Returns a 201 Created response
    /// with the created billing entry if successful; otherwise, returns an error response with the appropriate status
    /// code.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] IHandler<CreateBillingEntryCommand, BillingEntryDTO> handler,
        [FromBody] CreateBillingEntryCommand command,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Created($"/api/v1/billing/billingEntries/{response.Success.Id}", response.Success)
            : StatusCode(response.Failure.Code, response.Failure);
    }

    /// <summary>
    /// Retrieves a billing entry by its unique identifier.
    /// </summary>
    /// <remarks>Returns a 200 OK response with the billing entry if successful, or an error status code if
    /// the entry is not found or another failure occurs.</remarks>
    /// <param name="handler">The handler used to process the query for retrieving the billing entry. Must not be null.</param>
    /// <param name="id">The unique identifier of the billing entry to retrieve.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the billing entry if found; otherwise, an error response indicating
    /// the failure reason.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromServices] IHandler<GetBillingEntryByIdQuery, BillingEntryDTO> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new GetBillingEntryByIdQuery(id), HttpContext, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure.Code, response.Failure);
    }

    /// <summary>
    /// Handles an HTTP GET request to retrieve a collection of billing entries based on the specified query parameters.
    /// </summary>
    /// <remarks>The response will contain a collection of <see cref="BillingEntryDTO"/> objects on success.
    /// If the query is invalid or processing fails, an error response with details is returned. This endpoint supports
    /// cancellation via the provided token.</remarks>
    /// <param name="handler">The handler responsible for processing the billing entries query and returning the result. Must not be null.</param>
    /// <param name="query">The query parameters used to filter and paginate the list of billing entries. Cannot be null.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the list of billing entries if the operation succeeds; otherwise, an
    /// error response with the appropriate status code.</returns>
    [HttpGet]
    public async Task<IActionResult> List(
        [FromServices] IHandler<ListBillingEntriesQuery, IReadOnlyCollection<BillingEntryDTO>> handler,
        [FromQuery] ListBillingEntriesQuery query,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure.Code, response.Failure);
    }

    /// <summary>
    /// Updates an existing billing entry with the specified data.
    /// </summary>
    /// <param name="handler">The handler responsible for processing the update command and returning the result.</param>
    /// <param name="id">The unique identifier of the billing entry to update.</param>
    /// <param name="command">The command containing the updated billing entry data. Cannot be null.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the update operation.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the update operation. Returns a 200 OK response with the
    /// updated billing entry on success, or an error response with the appropriate status code on failure.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromServices] IHandler<UpdateBillingEntryCommand, BillingEntryDTO> handler,
        Guid id,
        [FromBody] UpdateBillingEntryCommand command,
        CancellationToken cancellationToken)
    {
        command.SetId(id);

        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure.Code, response.Failure);
    }
}