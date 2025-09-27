using QuizApp.Application.Abstractions.Storage;

namespace QuizApp.Infrastructure.Services.MinIo;

public class StorageKeyBuilder : IStorageKeyBuilder
{
    public string BuildUserPhotoKey(Guid userId)
    {
        return $"users/{userId:N}/photo";
    }
}
