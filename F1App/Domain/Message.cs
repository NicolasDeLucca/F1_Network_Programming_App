namespace Domain
{
    public class Message
    {
        public int Id { get; set; } 
        public string Text { get; set; }
        public bool Read { get; set; }
        public int AuthorId { get; }

        public Message(int authorId)
        {
            Read = false;
            AuthorId = authorId;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}