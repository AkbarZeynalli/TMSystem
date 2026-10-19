using TMSystem.Domain.Common;
using TMSystem.Domain.Exceptions;

namespace TMSystem.Domain.Entities;

public sealed class Attachment : AuditableEntity
{
    public const long MaxSizeInBytes = 10 * 1024 * 1024; // 10 MB

    public string FileName { get; private set; } = string.Empty;
    public string StoragePath { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeInBytes { get; private set; }
    public Guid TaskItemId { get; private set; }
    public Guid UploadedById { get; private set; }

    private Attachment() { } // EF Core üçün

    // Yalnız TaskItem.AddAttachment() vasitəsilə yaradılır
    internal static Attachment Create(
        Guid taskItemId, Guid uploadedById, string fileName, string storagePath, string contentType, long sizeInBytes)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("Faylın adı boş ola bilməz.");
        if (string.IsNullOrWhiteSpace(storagePath))
            throw new DomainException("Faylın yolu boş ola bilməz.");
        if (sizeInBytes <= 0)
            throw new DomainException("Fayl boş ola bilməz.");
        if (sizeInBytes > MaxSizeInBytes)
            throw new DomainException($"Faylın ölçüsü {MaxSizeInBytes / 1024 / 1024} MB-dan çox ola bilməz.");

        return new Attachment
        {
            TaskItemId = taskItemId,
            UploadedById = uploadedById,
            FileName = fileName.Trim(),
            StoragePath = storagePath,
            ContentType = contentType,
            SizeInBytes = sizeInBytes
        };
    }
}
