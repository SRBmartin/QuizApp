using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuizApp.Application.Abstractions.Identity;

namespace QuizApp.Api.Web.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireAdminRole : Attribute, IAsyncActionFilter
{
    private readonly string _role;
    public RequireAdminRole(string role = "Admin") => _role = role;

    public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        var user = ctx.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
        if (!user.IsAuthenticated || !string.Equals(user.Role, _role, StringComparison.OrdinalIgnoreCase))
        {
            ctx.Result = new ForbidResult();
            return;
        }
        await next();
    }
}
