namespace Sincera.Claims.Domain.Interfaces;

public interface IFileInspector
{
    Task<bool> IsFileCorruptedAsync(Stream file, CancellationToken ct);
}
