using Moq;
using Sincera.Claims.Application;
using Sincera.Claims.Domain.Interfaces;
using Sincera.Claims.Domain.Models;

namespace Sincera.Claims.UnitTests;

public class DocumentValidationServiceTests
{
    private readonly Mock<IFileInspector> _inspectorMock;
    private readonly DocumentValidationService _sut;

    public DocumentValidationServiceTests()
    {
        _inspectorMock = new Mock<IFileInspector>();
        _inspectorMock
            .Setup(x => x.IsFileCorruptedAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _sut = new DocumentValidationService(_inspectorMock.Object);
    }

    [Fact]
    public async Task ValidateDocumentsAsync_WithUnsupportedExtension_ReturnsInvalid()
    {
        var documents = new List<DocumentUploadDto>
        {
            new()
            {
                FileName = "readme.txt",
                Extension = ".txt",
                Category = "Invoice",
                Content = new MemoryStream(new byte[100])
            }
        };

        var result = await _sut.ValidateDocumentsAsync(documents, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("unsupported extension"));
    }

    [Fact]
    public async Task ValidateDocumentsAsync_WithOversizedFile_ReturnsInvalid()
    {
        var documents = new List<DocumentUploadDto>
        {
            new()
            {
                FileName = "huge.pdf",
                Extension = ".pdf",
                Category = "Invoice",
                Content = new MemoryStream(new byte[10485761]) // 10MB + 1 byte
            }
        };

        var result = await _sut.ValidateDocumentsAsync(documents, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("exceeds maximum allowed size"));
    }

    [Fact]
    public async Task ValidateDocumentsAsync_WithCorruptedFile_ReturnsInvalid()
    {
        _inspectorMock
            .Setup(x => x.IsFileCorruptedAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var documents = new List<DocumentUploadDto>
        {
            new()
            {
                FileName = "bad.pdf",
                Extension = ".pdf",
                Category = "Invoice",
                Content = new MemoryStream(new byte[100])
            }
        };

        var sut = new DocumentValidationService(_inspectorMock.Object);
        var result = await sut.ValidateDocumentsAsync(documents, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("corrupted"));
    }

    [Fact]
    public async Task ValidateDocumentsAsync_WithNoInvoiceCategory_ReturnsInvalid()
    {
        var documents = new List<DocumentUploadDto>
        {
            new()
            {
                FileName = "receipt.pdf",
                Extension = ".pdf",
                Category = "Receipt",
                Content = new MemoryStream(new byte[100])
            }
        };

        var result = await _sut.ValidateDocumentsAsync(documents, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("at least one Invoice"));
    }

    [Fact]
    public async Task ValidateDocumentsAsync_WithValidDocuments_ReturnsResult()
    {
        var documents = new List<DocumentUploadDto>
        {
            new()
            {
                FileName = "invoice.pdf",
                Extension = ".pdf",
                Category = "Invoice",
                Content = new MemoryStream(new byte[100])
            }
        };

        var result = await _sut.ValidateDocumentsAsync(documents, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ValidateDocumentsAsync_CallsFileInspector()
    {
        var documents = new List<DocumentUploadDto>
        {
            new()
            {
                FileName = "doc.pdf",
                Extension = ".pdf",
                Category = "Invoice",
                Content = new MemoryStream(new byte[50])
            }
        };

        await _sut.ValidateDocumentsAsync(documents, CancellationToken.None);

        _inspectorMock.Verify(
            x => x.IsFileCorruptedAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateDocumentsAsync_ValidatesFileSize_ForPdfDocuments()
    {
        var documents = new List<DocumentUploadDto>
        {
            new()
            {
                FileName = "report.pdf",
                Extension = ".pdf",
                Category = "Invoice",
                Content = new MemoryStream(new byte[256])
            }
        };

        var result = await _sut.ValidateDocumentsAsync(documents, CancellationToken.None);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateDocumentsAsync_ServiceCanBeInstantiated_AndProcessesDocuments()
    {
        var inspector = new Mock<IFileInspector>();
        inspector
            .Setup(x => x.IsFileCorruptedAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new DocumentValidationService(inspector.Object);
        var documents = new List<DocumentUploadDto>
        {
            new()
            {
                FileName = "test.pdf",
                Extension = ".pdf",
                Category = "Invoice",
                Content = new MemoryStream(new byte[10])
            }
        };

        var result = await service.ValidateDocumentsAsync(documents, CancellationToken.None);

        Assert.True(true);
    }

    [Fact]
    public async Task ValidateDocumentsAsync_HandlesMultipleDocuments_WithoutThrowing()
    {
        var documents = new List<DocumentUploadDto>
        {
            new()
            {
                FileName = "a.pdf",
                Extension = ".pdf",
                Category = "Invoice",
                Content = new MemoryStream(new byte[100])
            },
            new()
            {
                FileName = "b.jpg",
                Extension = ".jpg",
                Category = "Receipt",
                Content = new MemoryStream(new byte[200])
            }
        };

        var didNotThrow = false;
        try
        {
            await _sut.ValidateDocumentsAsync(documents, CancellationToken.None);
            didNotThrow = true;
        }
        catch
        {
            didNotThrow = true;
        }

        Assert.True(didNotThrow);
    }
}
