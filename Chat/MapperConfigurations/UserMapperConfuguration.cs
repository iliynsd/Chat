using AutoMapper;
using Chat.DTO;

namespace Chat.MapperConfigurations
{
    public class UserMapperConfuguration : Profile
    {
        public UserMapperConfuguration()
        {
            CreateMap<User, UserDto>();
        }
    }
}
