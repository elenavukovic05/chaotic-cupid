using CupidServer.Model;
using Microsoft.AspNetCore.SignalR;

namespace CupidServer.Hubs
{
    public class CupidHub : Hub
    {
        public readonly static Dictionary<string, Person> Users = new();

        public async Task InitSinglePerson(string username, string city, int age, string phoneNumber)
        {
            if (!Users.ContainsKey(username))
            { 
                Person p = new Person(username, city, age, phoneNumber, Context.ConnectionId);
                Users.Add(username, p);
                await Clients.Caller.SendAsync("RegistrationSuccessed", $"Uspjesno ste se registrovali kao {username}.");
                Console.WriteLine($"[SERVER] {username} se prijavio/la.");
            }
            else
            {
                await Clients.Caller.SendAsync("RegistrationError", $"Korisnicko ime '{username}' je vec zauzeto.");
                Console.WriteLine($"[SERVER] Pokusaj registracije sa zauzetim imenom: {username}");
            }
        }

        public async Task ConfirmReceived()
        {
            Person? p = Users.Values.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (p == null) return;
            p.WaitingConfirmation = false;
        }

        public async Task BlockUser(string username)
        {
            Person? p = Users.Values.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (p == null)
            {
                Console.WriteLine($"[SERVER] Korisnik {username} ne postoji");
                return;
            }

            if (!Users.ContainsKey(username))
            {
                await Clients.Caller.SendAsync("BlockError", $"Korisnik '{username}' ne postoji.");
                return;
            }

            if (username == p.Username)
            {
                await Clients.Caller.SendAsync("BlockError", "Ne mozete blokirati sami sebe.");
                return;
            }

            if (!p.BlockedUsernames.Contains(username))
            {
                p.BlockedUsernames.Add(username);
                await Clients.Caller.SendAsync("BlockSuccess", $"Uspjesno ste blokirali korisnika: {username}");
                Console.WriteLine($"[SERVER] {p.Username} blokirao/la: {username}");
            }
            
        }
    }
}
