using Microsoft.AspNetCore.Http;

namespace DatingAPI.Controllers.Helper
{
    public static class HelperException
    {
        public static void ApplicationError(this HttpResponse response,string message)
        {
            response.Headers.Add("Application-Error",message);
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");
        }
    }
}