using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Abstractions.Storage;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.User.Auth;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;
using System.Reflection.PortableExecutable;

namespace QuizApp.Application.Features.User.Register;

public class RegisterUserCommandHandler (
    IUserRepository userRepository,
    IPasswordService passwordService,
    IImageStorage imageStorage,
    IUnitOfWork uow,
    IImageBucketNameProvider bucketNameProvider,
    IStorageKeyBuilder storageKeyBuilder,
    IIdentityService identityService
) : IRequestHandler<RegisterUserCommand, Result<AuthDto>>
{
    public async Task<Result<AuthDto>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByUsernameAsync(command.Username, cancellationToken))
            return Result<AuthDto>.Failure(new Error("user.username_conflict", "Username already taken."));

        if (await userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
            return Result<AuthDto>.Failure(new Error("user.email_conflict", "Email already in use."));

        var hash = passwordService.Hash(command.Password);

        var user = Domain.Entities.User.Create(Guid.NewGuid(), command.Username, command.Email, hash, Role.User, null);

        string? photoUrl = null;
        if (command.ImageContent is not null)
        {
            var bucket = bucketNameProvider.GetUsersBucket();
            var objectKey = storageKeyBuilder.BuildUserPhotoKey(user.Id);
            var contentType = command.ImageContentType ?? "application/octet-stream";

            var objRef = await imageStorage.UploadAsync(bucket, objectKey, command.ImageContent, contentType, null, cancellationToken);

            user.Photo = $"{objRef.Bucket}/{objRef.Key}";

            photoUrl = imageStorage.TryBuildPublicUrl(objRef.Bucket, objRef.Key)!.ToString();
        }

        await userRepository.AddAsync(user, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        var token = await identityService.GenerateAccessTokenAsync(user.Id, user.Username, user.Role.ToString(), cancellationToken);

        return Result<AuthDto>.Success(new AuthDto(token));
    }
}
