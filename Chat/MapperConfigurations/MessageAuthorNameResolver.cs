using AutoMapper;
using Chat.DTO;

namespace Chat.MapperConfigurations
{
    public class MessageAuthorNameResolver : ITypeConverter<Message, MessageDto>
    {
        public MessageDto Convert(Message source, MessageDto destination, ResolutionContext context)
        {
            var authorName = context.Items["AuthorName"]?.ToString();
            destination = new MessageDto
            {
                Author = authorName,
                Text = source.Text,
                IsViewed = source.IsViewed,
                Time = source.Time
            };

            return destination;
        }
    }
}
