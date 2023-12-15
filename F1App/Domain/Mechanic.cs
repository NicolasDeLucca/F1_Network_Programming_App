using Exceptions;
using System.Collections.Generic;

namespace Domain
{
    public class Mechanic
    {
        private object _MessageLocker = new object();
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public List<Message> Messages { get; set; }

        public Mechanic()
        {
            Messages = new List<Message>();
        }

        public int AddMessage(Message message)
        {
            lock (_MessageLocker)
            {
                Message newMessage = new Message(message.AuthorId)
                {
                    Id = Messages.Count,
                    Text = message.Text,
                    Read = false
                };
                Messages.Add(newMessage);

                return newMessage.Id;
            }
        }

        public Message GetMessage(int id)
        {
            lock (_MessageLocker)
            {
                if (id < 0 || id >= Messages.Count || Messages[id] == null)
                    throw new ResourceNotFoundException("Specified message not found");

                return Messages[id];
            }
        }

        public void ReadMessage(int id)
        {
            GetMessage(id).Read = true;
        }
    }
}
