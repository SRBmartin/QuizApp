using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using QuizApp.Application.Abstractions.Storage;
using QuizApp.Application.DTOs.Storage;
using QuizApp.Infrastructure.Configuration;

namespace QuizApp.Infrastructure.Services.MinIo;

public class MinIoImageStorage : IImageStorage
{
    private readonly MinioImageStorageConfiguration _options;
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinIoImageStorage> _logger;

    public MinIoImageStorage(
        IOptions<MinioImageStorageConfiguration> options,
        ILogger<MinIoImageStorage> logger
    )
    {
        _logger = logger;
        _options = options.Value;

        var builder = new MinioClient()
            .WithEndpoint(_options.Endpoint)
            .WithCredentials(_options.AccessKey, _options.SecretKey)
            .WithSSL(_options.UseSsl);

        if (!string.IsNullOrWhiteSpace(_options.Region))
        {
            builder.WithRegion(_options.Region);
        }

        _minioClient = builder.Build();
    }

    /// <inheritdoc/>
    public async Task EnsureBucketAsync(string bucket, CancellationToken cancellationToken)
    {
        var exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket), cancellationToken);
        if (!exists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket), cancellationToken);

            _logger.LogInformation("Created bucket {Bucket}", bucket);
        }
    }

    /// <inheritdoc/>
    public async Task<ImageObjectRefDto> UploadAsync(string bucket, string objectKey, Stream content, string contentType, long? size, CancellationToken cancellationToken)
    {
        await EnsureBucketAsync(bucket, cancellationToken);

        var objectSize = size ?? (content.CanSeek ? content.Length : -1);

        var putArgs = new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectKey)
                .WithContentType(contentType)
                .WithStreamData(content)
                .WithObjectSize(objectSize);

        await _minioClient.PutObjectAsync(putArgs, cancellationToken);

        return new ImageObjectRefDto(bucket, objectKey);
    }

    /// <inheritdoc/>
    public async Task<Uri> GetPresignedDownloadUrlAsync(string bucket, string objectKey, TimeSpan expires, string? responseContentType = null, string? downloadFileName = null, CancellationToken cancellationToken = default)
    {
        var args = new PresignedGetObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectKey)
                .WithExpiry((int)expires.TotalSeconds);

        var responseHeaders = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(responseContentType))
            responseHeaders["response-content-type"] = responseContentType;

        if (!string.IsNullOrWhiteSpace(downloadFileName))
        {
            var safe = Uri.EscapeDataString(downloadFileName);
            responseHeaders["response-content-disposition"] =
                $"attachment; filename*=UTF-8''{safe}";
        }

        if (responseHeaders.Count > 0)
            args = args.WithHeaders(responseHeaders);

        var url = await _minioClient.PresignedGetObjectAsync(args);
        return new Uri(url, UriKind.Absolute);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string bucket, string objectKey, CancellationToken cancellationToken)
    {
        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucket).WithObject(objectKey), cancellationToken);
    }

    /// <inheritdoc/>
    public Uri? TryBuildPublicUrl(string bucket, string objectKey)
    {
        if (string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
            return null;

        var baseUri = new Uri(_options.PublicBaseUrl, UriKind.Absolute);
        var combined = new Uri(baseUri, $"{bucket}/{Uri.EscapeUriString(objectKey)}");

        return combined;
    }

}
