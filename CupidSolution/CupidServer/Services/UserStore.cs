using CupidServer.Model;

namespace CupidServer.Services
{
    public class UserStore
    {
        private readonly Dictionary<string, Person> _users = new();

        public bool TryAdd(Person person)
        {
            if (_users.ContainsKey(person.Username)) return false;
            _users[person.Username] = person;
            return true;
        }

        public Person? GetByConnectionId(string connectionId) =>
            _users.Values.FirstOrDefault(p => p.ConnectionId == connectionId);

        public Person? GetByUsername(string username) =>
            _users.TryGetValue(username, out var p) ? p : null;

        public bool Contains(string username) => _users.ContainsKey(username);

        public IEnumerable<Person> GetAll() => _users.Values;
    }
}
