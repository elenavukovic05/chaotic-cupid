namespace CupidServer.Model
{
    public class Person
    {
        public string Username { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public bool WaitingConfirmation { get; set; }
        public List<string> BlockedUsernames { get; set; } = new List<String>();
        public string ConnectionId { get; set; } = string.Empty;

        public Person()
        {

        }

        public Person(string username, string city, int age, string phoneNumber, string connectionId)
        {
            Username = username;
            City = city;
            Age = age;
            PhoneNumber = phoneNumber;
            ConnectionId = connectionId;
        }
    }
}
