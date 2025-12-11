namespace Shared
{
    public class Contact
    {
        public string Name { get; set; }
        public bool IsOnline { get; set; }

        public Contact(string name, bool isOnline = false)
        {
            Name = name;
            IsOnline = isOnline;
        }
    }
}
