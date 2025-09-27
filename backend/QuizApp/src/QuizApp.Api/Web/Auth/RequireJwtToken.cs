using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuizApp.Application.Abstractions.Identity;

namespace QuizApp.Api.Web.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireJwtToken : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        var user = ctx.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
        if (!user.IsAuthenticated || user.Id is null)
        {
            ctx.Result = new UnauthorizedResult();
            return;
        }
        await next();
    }
}
