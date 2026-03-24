using Microsoft.EntityFrameworkCore;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Remittances.DownloadRemittanceFile;

public class DownloadRemittanceFileCommandHandler(SpheraDbContext dbContext, [FromKeyedServices("billing")] IStorage storage, ILogger<DownloadRemittanceFileCommandHandler> logger) : IHandler<DownloadRemittanceFileCommand, (Stream, string)>
{
    public async Task<IResultDTO<(Stream, string)>> HandleAsync(DownloadRemittanceFileCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Baixando arquivo de billet {Id}", request.Id);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<(Stream, string)>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var entity = await dbContext.Remittances
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (entity is null)
            return ResultDTO<(Stream, string)>.AsFailure(new FailureDTO(404, "Remittance not found"));
        
        try
        {
            var fileName =
                $"remittances/{entity.FileName}";
                
            if (await storage.ExistsAsync(fileName, cancellationToken)) 
            {
                var result = await storage.DownloadAsync(fileName, cancellationToken);
                return ResultDTO<(Stream, string)>.AsSuccess((result, entity.FileName));
            }
            else 
                return ResultDTO<(Stream, string)>.AsFailure(new FailureDTO(404, "Remessa não encontrado"));
        }
        catch (DomainException ex)
        {
            return ResultDTO<(Stream, string)>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception)
        {
            return ResultDTO<(Stream, string)>.AsFailure(new FailureDTO(500, "Erro ao baixar remessa."));
        }
    }
}