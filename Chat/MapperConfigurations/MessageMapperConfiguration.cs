using AutoMapper;
using Chat.DTO;

namespace Chat.MapperConfigurations
{
    public class MessageMapperConfiguration : Profile
    {
        public MessageMapperConfiguration()
        {
            CreateMap<Message, MessageDto>().ConvertUsing<MessageAuthorNameResolver>();
        }
    }
}
