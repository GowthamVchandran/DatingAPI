using System;
using Microsoft.AspNetCore.Http;
using DatingAPI.Controllers.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        public static void AddPagination(this HttpResponse response, int currentPage, 
        int itemPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new paginationHeader(currentPage,itemPerPage,totalItems,totalPages);
            var camelcaseFormatter = new JsonSerializerSettings();
            camelcaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader,camelcaseFormatter));

            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
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