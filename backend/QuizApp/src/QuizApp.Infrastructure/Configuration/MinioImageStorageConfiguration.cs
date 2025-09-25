using System.ComponentModel.DataAnnotations;

namespace QuizApp.Infrastructure.Configuration;

public class MinioImageStorageConfiguration
{
    public const string SectionName = "MinioImageStorageConfiguration";

    [Required]
    public string Endpoint { get; set; } = default!;
    [Required]
    public bool UseSsl { get; set; } = false;
    [Required]
    public string AccessKey { get; set; } = default!;
    [Required]
    public string SecretKey { get; set; } = default!;
    [Required]
    public string PublicBaseUrl { get; set; } = default!;
    [Required]
    public string Region { get; set; } = default!;
    [Required]
    public bool ForcePathStyle { get; set; } = true;
    [Required]
    public long MultipartChunkSizeBytes { get; set; }
}
