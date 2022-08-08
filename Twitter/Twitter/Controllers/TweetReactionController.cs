using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twitter.Services;

namespace Twitter.Controllers
{
    [ApiController]
    [Route("api/Tweets/{username}")]
    [Authorize]
    public class TweetReactionController : ControllerBase
    {
        private ITweetReactionService tweetReactionService;
        public TweetReactionController(ITweetReactionService tweetReactionService)
        {
            this.tweetReactionService = tweetReactionService;
        }

        [HttpPost("{id:int}")]
        public async Task<IActionResult> Like(string username, int id)
        {
            var reactingUserName = User.Identity.Name;

            var response = await tweetReactionService.Like(username, id, reactingUserName);            

            if(!response.Success)
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }
        }
    }
}
