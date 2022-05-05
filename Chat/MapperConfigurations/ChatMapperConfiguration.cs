using AutoMapper;
using Chat.DTO;

namespace Chat.MapperConfigurations
{
    public class ChatMapperConfiguration : Profile
    {
        public ChatMapperConfiguration()
        {
            CreateMap<Chat, ChatDto>();
        }
    }
}