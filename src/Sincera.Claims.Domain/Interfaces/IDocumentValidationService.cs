using Sincera.Claims.Domain.Models;

namespace Sincera.Claims.Domain.Interfaces;

public interface IDocumentValidationService
{
    Task<DocumentValidationResult> ValidateDocumentsAsync(
        List<DocumentUploadDto> documents,
        CancellationToken cancellationToken);
}
