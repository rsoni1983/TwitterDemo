using Twitter.Dto;
using Twitter.Models;

namespace Twitter.Services
{
    public interface ITweetService
    {
        Task<ServiceResponse<GetTweetDto>> AddTweet(string username, AddTweetDto addTweetDto);
        Task<ServiceResponse<List<GetTweetDto>>> DeleteTweet(string username, int id);
        Task<ServiceResponse<List<GetTweetDto>>> GetTweets();
        Task<ServiceResponse<List<GetTweetDto>>> GetTweets(string userName);
        Task<ServiceResponse<GetTweetDto>> ReplyTweet(string username, string reactingUserName, int id, AddTweetDto addTweetDto);
        Task<ServiceResponse<GetTweetDto>> UpdateTweet(string username, int id, UpdateTweetDto updateTweetDto);
    }
}