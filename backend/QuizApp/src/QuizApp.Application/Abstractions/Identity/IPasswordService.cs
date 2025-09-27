﻿namespace QuizApp.Application.Abstractions.Identity;

public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
