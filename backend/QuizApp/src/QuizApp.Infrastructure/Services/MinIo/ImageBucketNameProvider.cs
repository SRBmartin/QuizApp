using Microsoft.Extensions.Options;
using QuizApp.Application.Abstractions.Storage;
using QuizApp.Infrastructure.Configuration;

namespace QuizApp.Infrastructure.Services.MinIo;

public class ImageBucketNameProvider (
    IOptions<MinioImageStorageConfiguration> options    
) : IImageBucketNameProvider
{
    private readonly MinioImageStorageConfiguration _options = options.Value;

    public string GetUsersBucket() => _options.UsersBucket;

}
