using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Twitter.Dto;
using Twitter.Models;

namespace Twitter.Services
{
    public class TweetService : ITweetService
    {
        private ApplicationDbContext context;
        private IMapper mapper;

        public TweetService(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            this.context = applicationDbContext;
            this.mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetTweetDto>>> GetTweets()
        {
            var result = context.Tweets.Include("User").ToList();
            var tweets = mapper.Map<List<GetTweetDto>>(result);

            return new ServiceResponse<List<GetTweetDto>>
            {
                Data = tweets
            };
        }

        public async Task<ServiceResponse<List<GetTweetDto>>> GetTweets(string userName)
        {
            var response = new ServiceResponse<List<GetTweetDto>>();
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            if(user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            var tweets = mapper.Map<List<GetTweetDto>>(await context.Tweets.Include("User").Where(x => x.UserId == user.Id).ToListAsync());

            response.Data = tweets;

            return response;
        }

        public async Task<ServiceResponse<GetTweetDto>> AddTweet(string username, AddTweetDto addTweetDto)
        {
            if(addTweetDto.ParentTweetId == 0)
            {
                addTweetDto.ParentTweetId = null;
            }

            var response = new ServiceResponse<GetTweetDto>();
            var tweet = mapper.Map<Tweet>(addTweetDto);
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            tweet.UserId = user.Id;
            context.Tweets.Add(tweet);
            await context.SaveChangesAsync();

            tweet.User = user;

            return new ServiceResponse<GetTweetDto>
            {
                Data = mapper.Map<GetTweetDto>(tweet)
            };
        }

        public async Task<ServiceResponse<GetTweetDto>> AddTweet(string username, int id, AddTweetDto addTweetDto)
        {
            if (addTweetDto.ParentTweetId == 0)
            {
                addTweetDto.ParentTweetId = null;
            }

            var response = new ServiceResponse<GetTweetDto>();
            var tweet = mapper.Map<Tweet>(addTweetDto);
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            tweet.UserId = user.Id;
            context.Tweets.Add(tweet);
            await context.SaveChangesAsync();

            tweet.User = user;

            return new ServiceResponse<GetTweetDto>
            {
                Data = mapper.Map<GetTweetDto>(tweet)
            };
        }

        public async Task<ServiceResponse<GetTweetDto>> ReplyTweet(string username, string reactingUserName, int id, AddTweetDto addTweetDto)
        {
            var response = new ServiceResponse<GetTweetDto>();

            var tweet = mapper.Map<Tweet>(addTweetDto);
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == reactingUserName);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            if (tweet.ParentTweetId == null || tweet.ParentTweetId == 0 || tweet.ParentTweetId != id)
            {
                response.Success = false;
                response.Message = "Invalid parent tweet id";
                return response;
            }

            var parentTweet = await context.Tweets.Include("User").FirstOrDefaultAsync(x => x.Id == tweet.ParentTweetId && x.User.UserName == username);

            if(parentTweet == null)
            {
                response.Success = false;
                response.Message = "Parent tweet not found";
                return response;
            }

            tweet.UserId = user.Id;
            context.Tweets.Add(tweet);
            await context.SaveChangesAsync();

            tweet.User = user;
            response.Data = mapper.Map<GetTweetDto>(tweet);

            return response;
        }

        public async Task<ServiceResponse<GetTweetDto>> UpdateTweet(string username, int id, UpdateTweetDto updateTweetDto)
        {
            if(updateTweetDto.ParentTweetId == 0)
            {
                updateTweetDto.ParentTweetId = null;
            }

            var response = new ServiceResponse<GetTweetDto>();

            if (id != updateTweetDto.Id)
            {
                response.Success = false;
                response.Message = "Incorrect tweet id";
                return response;
            }

            var tweet = mapper.Map<Tweet>(updateTweetDto);
            var result = await context.Tweets.Include("User").AsNoTracking().FirstOrDefaultAsync(x => x.Id == tweet.Id && x.User.UserName == username);

            if (result == null)
            {
                response.Success = false;
                response.Message = "Tweet not found";
                return response;
            }

            tweet.UserId = result.UserId;
            tweet.User = result.User;

            context.Entry(tweet).State = EntityState.Modified;
            await context.SaveChangesAsync();

            response.Data = mapper.Map<GetTweetDto>(tweet);

            return response;
        }

        public async Task<ServiceResponse<List<GetTweetDto>>> DeleteTweet(string username, int id)
        {
            var response = new ServiceResponse<List<GetTweetDto>>();

            var result = await context.Tweets.Include("User").FirstOrDefaultAsync(x => x.Id == id && x.User.UserName == username);

            if (result == null)
            {
                response.Success = false;
                response.Message = "Tweet not found";
                return response;
            }

            context.Tweets.Remove(result);
            await context.SaveChangesAsync();

            response.Data = mapper.Map<List<GetTweetDto>>(await context.Tweets.Where(x=> x.UserId == result.UserId).ToListAsync());

            return response;
        }
    }
}
