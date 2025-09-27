namespace QuizApp.Application.Abstractions.Storage;

public interface IStorageKeyBuilder
{
    string BuildUserPhotoKey(Guid userId);
}
