namespace Sincera.Claims.Domain.Models;

public class DocumentValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}
