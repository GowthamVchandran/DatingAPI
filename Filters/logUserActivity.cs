using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingAPI.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingAPI.Filters
{
    public class logUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var  result = await next();

            var userID = int.Parse(result.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var service = result.HttpContext.RequestServices.GetService<IDatingRepository>();

            var findUserLastActiveDate = await service.GetUser(userID);

            findUserLastActiveDate.LastActive =DateTime.Now;

            await service.SaveAll();
        }
    }
}