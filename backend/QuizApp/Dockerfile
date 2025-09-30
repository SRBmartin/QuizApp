FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ./backend/QuizApp/src/QuizApp.Api/QuizApp.Api.csproj ./backend/QuizApp/src/QuizApp.Api/
COPY ./backend/QuizApp/src/QuizApp.Application/QuizApp.Application.csproj ./backend/QuizApp/src/QuizApp.Application/
COPY ./backend/QuizApp/src/QuizApp.Infrastructure/QuizApp.Infrastructure.csproj ./backend/QuizApp/src/QuizApp.Infrastructure/
COPY ./backend/QuizApp/src/QuizApp.Domain/QuizApp.Domain.csproj ./backend/QuizApp/src/QuizApp.Domain/

RUN dotnet restore ./backend/QuizApp/src/QuizApp.Api/QuizApp.Api.csproj --nologo

COPY ./backend/QuizApp ./backend/QuizApp

RUN dotnet publish ./backend/QuizApp/src/QuizApp.Api/QuizApp.Api.csproj \
    -c Release -o /app/publish /p:UseAppHost=false --nologo

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "QuizApp.Api.dll"]
