namespace Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Category() { }

        public override string ToString()
        {
            return "Name : " + Name; 
        }
    }
}
