using System;

namespace DatingAPI.Dtos
{
    public class MessageForCreatingDto
    {
        public MessageForCreatingDto()
        {
            MessageSent = DateTime.Now;
        }
        public int SenderId { get; set; }
        public int RecipientId  { get; set; }
        public DateTime MessageSent { get; set; }

        public string Content { get; set; }
    }
}