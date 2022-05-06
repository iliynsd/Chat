using AutoMapper;
using Chat.DTO;
using System;
using System.Collections.Generic;

namespace Chat.MapperConfigurations
{
    public class MessageMapperConfiguration : Profile
    {
        public MessageMapperConfiguration()
        {
            CreateMap<(Message mes, User user), MessageDto>().ForMember(mes => mes.IsViewed, opt => opt.MapFrom(x => x.mes.IsViewed)).ForMember(mes => mes.Text, opt => opt.MapFrom(x => x.mes.Text)).ForMember(mes => mes.Time, opt => opt.MapFrom(x => x.mes.Time)).ForMember(mes => mes.Author, opt => opt.MapFrom(x => x.user.Name)); 
        }
    }
}