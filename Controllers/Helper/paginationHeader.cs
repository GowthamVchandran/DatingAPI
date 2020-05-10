namespace DatingAPI.Controllers.Helper
{
    public class paginationHeader
    {
        public int CurrentPage { get; set; }
         public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPage { get; set; }
        public paginationHeader(int currentpage, int itemsPerPage, int totalItems, int totalPage)
        {
            this.CurrentPage =currentpage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalItems = totalItems;
            this.TotalPage = totalPage;

        }
    }
}