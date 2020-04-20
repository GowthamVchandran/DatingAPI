using System;
using Microsoft.AspNetCore.Http;

namespace DatingAPI.Controllers.Helper
{
    public static class HelperException
    {
        public static void ApplicationError(this HttpResponse response,string message)
        {
            // Intercept the http Request
            response.Headers.Add("Application-Error",message);
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");
        }

        public static int CalculateAge(this DateTime thedatetime)
        {
            var age = DateTime.Today.Year - thedatetime.Year;
            
            if(thedatetime.AddYears(age)>DateTime.Today)
              age--;

              return age;
        }
    }
}