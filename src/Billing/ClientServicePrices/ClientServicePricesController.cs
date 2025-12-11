using Microsoft.AspNetCore.Mvc;
using Sphera.API.Billing.ClientServicePrices.CreateClientServicePrice;
using Sphera.API.Billing.ClientServicePrices.GetClientServicePriceById;
using Sphera.API.Billing.ClientServicePrices.ListClientServicePrices;
using Sphera.API.Shared.Interfaces;

namespace Sphera.API.Billing.ClientServicePrices;

[ApiController]
[Route("api/v1/billing/[controller]")]
public class ClientServicePricesController : ControllerBase
{
    /// <summary>
    /// Creates a new client service price using the specified command and returns the result of the operation.
    /// </summary>
    /// <param name="handler">The handler responsible for processing the <paramref name="command"/> and returning the result.</param>
    /// <param name="command">The command containing the details required to create a new client service price. Cannot be null.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the creation operation. Returns a 201 Created response
    /// with the created resource if successful; otherwise, returns an error response with the appropriate status code.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] IHandler<CreateClientServicePriceCommand, ClientServicePriceDTO> handler,
        [FromBody] CreateClientServicePriceCommand command,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(command, HttpContext, cancellationToken);

        return response.IsSuccess
            ? Created($"/api/v1/billing/clientServicePrices/{response.Success.Id}", response.Success)
            : StatusCode(response.Failure.Code, response.Failure);
    }

    /// <summary>
    /// Retrieves the client service price details for the specified identifier.
    /// </summary>
    /// <remarks>Returns a 200 OK response with the client service price details if the operation succeeds, or
    /// an error response with the appropriate status code if it fails.</remarks>
    /// <param name="handler">The handler used to process the query for retrieving client service price information.</param>
    /// <param name="id">The unique identifier of the client service price to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the client service price details if found; otherwise, an error
    /// response indicating the failure reason.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromServices] IHandler<GetClientServicePriceByIdQuery, ClientServicePriceDTO> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new GetClientServicePriceByIdQuery(id), HttpContext, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure.Code, response.Failure);
    }

    /// <summary>
    /// Handles HTTP GET requests to retrieve a collection of client service prices based on the specified query
    /// parameters.
    /// </summary>
    /// <param name="handler">The handler responsible for processing the query and returning the collection of client service prices. Must not
    /// be null.</param>
    /// <param name="query">The query parameters used to filter and specify which client service prices to retrieve. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the collection of client service prices if the operation succeeds;
    /// otherwise, an error response with the appropriate status code.</returns>
    [HttpGet]
    public async Task<IActionResult> List(
        [FromServices] IHandler<ListClientServicePricesQuery, IReadOnlyCollection<ClientServicePriceDTO>> handler,
        [FromQuery] ListClientServicePricesQuery query,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(query, HttpContext, cancellationToken);
        return response.IsSuccess ? Ok(response.Success) : StatusCode(response.Failure.Code, response.Failure);
    }
}