namespace DatingAPI.Controllers.Helper
{
    public class MessageParams
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

        public string MessageContainer { get; set; } ="Unread";
    }
}