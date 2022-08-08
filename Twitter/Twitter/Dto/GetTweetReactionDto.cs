namespace Twitter.Dto
{
    public class GetTweetReactionDto
    {

        public int Id { get; set; }
        public int TweetId { get; set; }
        public int UserId { get; set; }
        public int ReactionType { get; set; }
    }
}
