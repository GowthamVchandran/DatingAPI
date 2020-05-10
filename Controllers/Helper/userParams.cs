namespace DatingAPI.Controllers.Helper
{
    public class userParams
    {
        public int PageNumber { get; set; }=1;

        public const int MaxPageSize = 50;

        private int pageSize = 5;
        public int PageSize { 
        get
        { 
           return pageSize;
        } 
        set 
        { 
         pageSize = (value > MaxPageSize)? MaxPageSize : value; 
        }        
    }
        public int UserId { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
         public string Orderby { get; set; }
         public bool Likers {get;set;}=false;
         public bool Likees {get;set;}=false;
    }
}