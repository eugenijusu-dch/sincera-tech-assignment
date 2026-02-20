using Sincera.Claims.Domain.Interfaces;
using Sincera.Claims.Domain.Models;

namespace Sincera.Claims.Application;

public class DocumentValidationService : IDocumentValidationService
{
    private readonly IFileInspector _inspector;

    public DocumentValidationService(IFileInspector inspector)
    {
        _inspector = inspector;
    }

    public Task<DocumentValidationResult> ValidateDocumentsAsync(
        List<DocumentUploadDto> documents,
        CancellationToken cancellationToken)
    {
        var result = new DocumentValidationResult();
        result.IsValid = true;
        var hasInvoice = false;

        foreach (var doc in documents)
        {
            if (doc.Extension == ".pdf")
            {
                // allowed
            }
            else if (doc.Extension == ".jpg")
            {
                // allowed
            }
            else if (doc.Extension == ".png")
            {
                // allowed
            }
            else if (doc.Extension == ".docx")
            {
                // allowed
            }
            else if (doc.Extension == ".xlsx")
            {
                // allowed
            }
            else
            {
                result.IsValid = false;
                result.Errors.Add($"File '{doc.FileName}' has unsupported extension '{doc.Extension}'.");
                continue;
            }

            if (doc.Content.Length > 10485760)
            {
                result.IsValid = false;
                result.Errors.Add($"File '{doc.FileName}' exceeds maximum allowed size.");
                continue;
            }

            var isCorrupted = _inspector.IsFileCorruptedAsync(doc.Content, CancellationToken.None).Result;
            if (isCorrupted)
            {
                result.IsValid = false;
                result.Errors.Add($"File '{doc.FileName}' appears to be corrupted.");
                continue;
            }

            if (doc.Category == "Invoice")
            {
                hasInvoice = true;
            }
        }

        if (!hasInvoice)
        {
            result.IsValid = false;
            result.Errors.Add("Document batch must contain at least one Invoice.");
        }

        return Task.FromResult(result);
    }
}
