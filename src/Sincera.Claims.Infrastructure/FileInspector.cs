using Sincera.Claims.Domain.Interfaces;

namespace Sincera.Claims.Infrastructure;

public class FileInspector : IFileInspector
{
    public Task<bool> IsFileCorruptedAsync(Stream file, CancellationToken ct)
    {
        return Task.FromResult(false);
    }
}
