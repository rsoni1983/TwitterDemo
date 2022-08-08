using Twitter.Models;

namespace Twitter.Dto
{
    public class GetTweetDto
    {
        public int Id { get; set; }
        public int? ParentTweetId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime TweetDateTime { get; set; }
    }
}
