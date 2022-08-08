using AutoMapper;
using Twitter.Dto;
using Twitter.Models;

namespace Twitter.AutoMapperProfile
{
    public class AppProfile: Profile
    {
        public AppProfile()
        {
            CreateMap<GetUserDto, User>()
                .ReverseMap();

            CreateMap<GetTweetDto, Tweet>()
                .ReverseMap()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<GetTweetReactionDto, TweetReaction>()
                .ReverseMap();

            CreateMap<AddTweetDto, Tweet>()
                .ReverseMap();

            CreateMap<UpdateTweetDto, Tweet>()
                .ReverseMap();
        }
    }
}
