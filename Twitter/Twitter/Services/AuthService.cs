using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Twitter.Dto;
using Twitter.Models;

namespace Twitter.Services
{
    public class AuthService : IAuthService
    {
        private ApplicationDbContext context;
        private IMapper mapper;
        private IConfiguration configuration;
        public AuthService(ApplicationDbContext applicationDbContext, IMapper mapper, IConfiguration configuration)
        {
            this.context = applicationDbContext;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public async Task<ServiceResponse<List<GetUserDto>>> GetUsers()
        {
            var response = new ServiceResponse<List<GetUserDto>>()
            {
                Data = mapper.Map<List<GetUserDto>>(context.Users.ToList())
            };

            return response;
        }

        public async Task<ServiceResponse<List<GetUserDto>>> Search(string username)
        {
            var response = new ServiceResponse<List<GetUserDto>>()
            {
                Data = mapper.Map<List<GetUserDto>>(context.Users.Where(x => x.UserName.ToLower().Equals(username.ToLower())))
            };

            return response;
        }

        public async Task<ServiceResponse<bool>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var response = new ServiceResponse<bool>();

            var user = context.Users.AsNoTracking().FirstOrDefault(x => x.UserName == resetPasswordDto.UserName);

            if (user == null)
            {
                response.Data = false;
                response.Success = false;
                response.Message = "User not found";
            }
            else
            {
                CreatePassword(resetPasswordDto.Password, out byte[] passwordSalt, out byte[] passwordHash);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                context.Entry(user).State = EntityState.Modified;
                await context.SaveChangesAsync();
                response.Data = true;
            }

            return response;
        }

        public async Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto registerUserDto)
        {
            var response = new ServiceResponse<GetUserDto>();

            if (await UserExists(registerUserDto.UserName))
            {
                response.Success = false;
                response.Message = "User already exixts";
                return response;
            }

            CreatePassword(registerUserDto.Password, out byte[] passwordSalt, out byte[] passwordHash);

            var user = new User
            {
                UserName = registerUserDto.UserName,
                TwitterHandle = registerUserDto.TwitterHandle,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            response.Success = true;
            response.Data = mapper.Map<GetUserDto>(user);

            return response;
        }

        public async Task<ServiceResponse<string>> Login(LoginDto loginDto)
        {
            var response = new ServiceResponse<string>();

            var user = context.Users.FirstOrDefault(x => x.UserName == loginDto.UserName);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
            }
            else if (!VerifyPassword(loginDto.Password, user.PasswordSalt, user.PasswordHash))
            {
                response.Success = false;
                response.Message = "Password not found";
            }
            else
            {
                var token = CreateToken(user);
                response.Data = token;
            }

            return response;
        }

        private string CreateToken(User user)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = creds,
                Expires = DateTime.Now.AddDays(1)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private async Task<bool> UserExists(string username)
        {
            if (await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePassword(string password, out byte[] passwordSalt, out byte[] passwordHash)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
