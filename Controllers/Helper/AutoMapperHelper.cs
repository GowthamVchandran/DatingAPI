using System.Linq;
using AutoMapper;
using DatingAPI.Dtos;
using DatingAPI.Model;

namespace DatingAPI.Controllers.Helper
{
    public class AutoMapperHelper:Profile
    {
        public AutoMapperHelper()
        {
            CreateMap<User,UsersForDto>()
             .ForMember(dest => dest.PhotoURL, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(
                 p => p.IsMain
             ).Url ))
             .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DOB.CalculateAge()));

            CreateMap<User,UserDetailDto>()
               .ForMember(dest => dest.PhotoURL, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(
                 p => p.IsMain
             ).Url ))
              .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DOB.CalculateAge()));
              
            CreateMap<Photo,PhotoDetailDto>();
        }
    }
}