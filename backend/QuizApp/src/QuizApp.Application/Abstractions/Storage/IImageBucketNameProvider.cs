namespace QuizApp.Application.Abstractions.Storage;

public interface IImageBucketNameProvider
{
    string GetUsersBucket();
}
