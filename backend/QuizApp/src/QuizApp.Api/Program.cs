using QuizApp.Api.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
