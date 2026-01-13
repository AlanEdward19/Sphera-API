using Microsoft.EntityFrameworkCore;
using Sphera.API.Billing.Billets.Enums;
using Sphera.API.External.Database;
using Sphera.API.Shared;
using Sphera.API.Shared.DTOs;
using Sphera.API.Shared.Interfaces;
using Sphera.API.Shared.Utils;

namespace Sphera.API.Billing.Remittances.GenerateRemittanceFile;

public class GenerateRemittanceFileCommandHandler(
    SpheraDbContext dbContext,
    [FromKeyedServices("billing")] IStorage storage,
    ILogger<GenerateRemittanceFileCommandHandler> logger)
    : IHandler<GenerateRemittanceFileCommand, bool>
{
    public async Task<IResultDTO<bool>> HandleAsync(GenerateRemittanceFileCommand request, HttpContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Gerando arquivo de remessa {Id}", request.Id);

        var userId = context.User.GetUserId();
        if (userId == Guid.Empty)
            return ResultDTO<bool>.AsFailure(new FailureDTO(401, "Unauthorized"));
        
        var entity = await dbContext.Remittances
            .Include("Billets")
            .Include("Billets.Client")
            .Include("Billets.Installment")
            .Include("Billets.Installment.Invoice")
            .Include(r => r.Configuration)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (entity is null)
            return ResultDTO<bool>.AsFailure(new FailureDTO(404, "Remittance not found"));
        
        try
        {
            var configurationRemittances = await dbContext.Remittances
                .AsNoTracking()
                .Where(b => entity.ConfigurationId == b.ConfigurationId && !string.IsNullOrWhiteSpace(b.FileName))
                .ToListAsync(cancellationToken);

            var date = DateTime.Now;
            var fileName = $"CB{date:ddMM}{(configurationRemittances.Count + 1).ToString("##").PadLeft(2, '0')}.REM";
            entity.UpdateFileName(fileName, userId);
            
            if (entity.Configuration == null)
                return ResultDTO<bool>.AsFailure(new FailureDTO(500, "Erro ao gerar arquivo de remessa."));

            var fileData = entity.Bank == EBilletBank.Bradesco 
                ? BradescoFileGenerator.GenerateRemmitanceFile(entity.Billets.ToList(), entity.Configuration.StartingSequentialNumber + configurationRemittances.Count + 1)
                : SicoobFileGenerator.GenerateRemmitanceFile(entity.Billets.ToList());
            var storageFileName = $"remittances/{fileName}";

            if (await storage.ExistsAsync(fileName, cancellationToken))
                await storage.DeleteAsync(fileName, cancellationToken);

            await storage.UploadAsync(fileData, storageFileName, "text/plain", cancellationToken);

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
