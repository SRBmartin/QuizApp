using QuizApp.Api.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuizApp API v1");
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseSerilogRequestLogging();

app.UseCors(Cors.Name);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
