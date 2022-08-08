using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Twitter.Dto;
using Twitter.Models;

namespace Twitter.Services
{
    public class TweetReactionService : ITweetReactionService
    {
        private ApplicationDbContext context;
        private IMapper mapper;

        public TweetReactionService(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            this.context = applicationDbContext;
            this.mapper = mapper;
        }

        public async Task<ServiceResponse<GetTweetDto>> Like(string userName, int tweetId, string reactingUserName)
        {
            var response = new ServiceResponse<GetTweetDto>();
            var tweet = await context.Tweets.Include("TweetReactions").Include("User").FirstOrDefaultAsync(x => x.Id == tweetId && x.User.UserName == userName);
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == reactingUserName);

            if (tweet == null || user == null)
            {
                response.Success = false;
                response.Message = "Tweet or user not found";
                return response;
            }

            var tweetReaction = new TweetReaction
            {
                UserId = user.Id,
                TweetId = tweetId,
                ReactionType = (int)ReactionType.Thumsup
            };

            context.TweetReactions.Add(tweetReaction);
            await context.SaveChangesAsync();

            response.Data = mapper.Map<GetTweetDto>(tweet);

            return response;
        }
    }
}
