using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twitter.Dto;
using Twitter.Services;

namespace Twitter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TweetsController : ControllerBase
    {
        private ITweetService tweetService;

        public TweetsController(ITweetService tweetService)
        {
            this.tweetService = tweetService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await tweetService.GetTweets());
        }

        [HttpGet("{username}")]
        [Authorize]
        public async Task<IActionResult> GetTweets(string username)
        {
            var response = await tweetService.GetTweets(username);

            if(!response.Success)
            {
                return NotFound(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPost("{username}/add/")]
        [Authorize]
        public async Task<IActionResult> AddTweet(string username, AddTweetDto addTweetDto)
        {
            var response = await tweetService.AddTweet(username, addTweetDto);

            if (!response.Success)
            {
                return NotFound(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPut("{username}/update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTweet(string username, int id, UpdateTweetDto updateTweetDto)
        {
            var response = await tweetService.UpdateTweet(username, id, updateTweetDto);

            if (!response.Success)
            {
                return NotFound(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPost("{username}/reply/{id}")]
        [Authorize]
        public async Task<IActionResult> Reply(string username, int id, AddTweetDto addTweetDto)
        {
            var reactingUserName = User.Identity.Name;
            var response = await tweetService.ReplyTweet(username, id, addTweetDto);

            if (!response.Success)
            {
                return NotFound(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpDelete("{username}/delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTweet(string username, int id)
        {
            var response = await tweetService.DeleteTweet(username, id);

            if (!response.Success)
            {
                return NotFound(response);
            }
            else
            {
                return Ok(response);
            }
        }

    }
}
