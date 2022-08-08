using System.ComponentModel.DataAnnotations;

namespace Twitter.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        [Required]
        [StringLength(255)]
        public string TwitterHandle { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public virtual List<Tweet> Tweets { get; set; }
        public virtual List<TweetReaction> TweetReactions { get; set; }
    }
}
