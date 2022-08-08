using Twitter.Dto;
using Twitter.Models;

namespace Twitter.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<List<GetUserDto>>> GetUsers();
        Task<ServiceResponse<string>> Login(LoginDto loginDto);
        Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto registerUserDto);
        Task<ServiceResponse<bool>> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<ServiceResponse<List<GetUserDto>>> Search(string username);
    }
}