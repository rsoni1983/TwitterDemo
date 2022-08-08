using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twitter.Dto;
using Twitter.Services;

namespace Twitter.Controllers
{
    [Route("api/Tweets/Users")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService authService;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            var response = await authService.Register(registerUserDto);

            if(!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var response = await authService.Login(loginDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPost("ResetPassword")]
        [Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var response = await authService.ResetPassword(resetPasswordDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await authService.GetUsers();

            return Ok(response);
        }

        [HttpGet("Search/{username}")]
        [Authorize]
        public async Task<IActionResult> Search(string username)
        {
            var response = await authService.Search(username);

            return Ok(response);
        }

    }
}
