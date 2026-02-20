namespace Sincera.Claims.Domain.Models;

public class DocumentUploadDto
{
    public string FileName { get; set; }
    public string Extension { get; set; } // e.g. ".pdf"
    public string Category { get; set; }  // e.g. "Invoice", "Receipt"
    public Stream Content { get; set; }
}
