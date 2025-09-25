using QuizApp.Application.DTOs.Storage;

namespace QuizApp.Application.Abstractions.Storage;

public interface IImageStorage
{
    /// <summary>
    /// Ensures that the specified bucket exists. If it does not exist, it will be created.
    /// This method is idempotent.
    /// </summary>
    /// <param name="bucket">Bucket name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task EnsureBucketAsync(string bucket, CancellationToken cancellationToken);

    /// <summary>
    /// Direct MinIo upload.
    /// </summary>
    Task<ImageObjectRefDto> UploadAsync(
        string bucket,
        string objectKey,
        Stream content,
        string contentType,
        long? size,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Generate time-limited URL for browser download/view.
    /// </summary>
    Task<Uri> GetPresignedDownloadUrlAsync(
        string bucket,
        string objectKey,
        TimeSpan expires,
        string? responseContentType = null,
        string? downloadFileName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete object.
    /// </summary>
    Task DeleteAsync(string bucket, string objectKey, CancellationToken cancellationToken);

    /// <summary>
    /// If the bucket has a public policy, this composes a stable URL; otherwise null.
    /// </summary>
    Uri? TryBuildPublicUrl(string bucket, string objectKey);
}
