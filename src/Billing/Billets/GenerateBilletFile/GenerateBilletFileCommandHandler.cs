using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Billets.Enums;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Billets.GenerateBilletFile;

public class GenerateBilletFileCommandHandler(SpheraDbContext dbContext, [FromKeyedServices("billing")] IStorage storage, ILogger<GenerateBilletFileCommandHandler> logger) : IHandler<GenerateBilletFileCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(GenerateBilletFileCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Gerando arquivo de boleto {Id}", request.Id);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<bool>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var entity = await dbContext.Billets
            .Include(b => b.Client)
            .Include(b => b.Installment)
            .ThenInclude(i => i.Invoice)
            .Include(r => r.Configuration)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (entity is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Billet not found"));
        
        try
        {
            if (entity.Configuration == null)
                return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao gerar arquivo de remessa."));

            var fileData = entity.Bank == EBilletBank.Bradesco 
                ? BradescoFileGenerator.GenerateBilletFile(entity)
                : SicoobFileGenerator.GenerateBilletFile(entity);
            var fileName = $"billets/{entity.ClientId}/{entity.Id}.pdf";

            if (await storage.ExistsAsync(fileName, cancellationToken))
                await storage.DeleteAsync(fileName, cancellationToken);

            await storage.UploadAsync(fileData, fileName, "text/plain", cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return ResultDTO<bool>.AsSuccess(true);
        }
        catch (DomainException ex)
        {
            return ResultDTO<bool>.AsFailure(new FailureDTO(400, ex.Message));
        }
        catch (Exception)
        {
            return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao gerar remessa."));
        }
    }
}

