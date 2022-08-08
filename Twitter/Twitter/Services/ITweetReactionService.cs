using Twitter.Dto;
using Twitter.Models;

namespace Twitter.Services
{
    public interface ITweetReactionService
    {
        Task<ServiceResponse<GetTweetDto>> Like(string userName, int tweetId, string reactingUserName);
    }
}