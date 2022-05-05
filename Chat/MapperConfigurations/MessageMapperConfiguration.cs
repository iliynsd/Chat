using AutoMapper;
using Chat.DTO;
using System.Collections.Generic;

namespace Chat.MapperConfigurations
{
    public class MessageMapperConfiguration : Profile
    {
        public MessageMapperConfiguration()
        {
            CreateMap<Message, MessageDto>();
            
        }
    }
}
