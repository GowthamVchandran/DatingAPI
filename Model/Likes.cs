namespace DatingAPI.Model
{
    public class Likes
    {
        public int LikerID { get; set; }
        public int LikeeID { get; set; }
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}