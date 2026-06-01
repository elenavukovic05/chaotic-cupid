using CupidServer.Hubs;
using CupidServer.Model;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using System.Security.Cryptography;

namespace CupidServer.Services
{
    public class CupidService : BackgroundService
    {
        private readonly IHubContext<CupidHub> _hubContext;
        private readonly UserStore _userStore;

        public CupidService(IHubContext<CupidHub> hubContext, UserStore userStore)
        {
            _hubContext = hubContext;
            _userStore = userStore;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                await SendLetters();
            }
        }

        private async Task SendLetters() {
            List<string> messages = new List<string>
            {
                "Radujem se nasem susretu!",
                "Zelim da se upoznamo.",
                "Nisam zainteresovan/a za upoznavanje."
            };

            foreach (var receiver in _userStore.GetAll())
            {
                if (receiver.WaitingConfirmation) continue;

                Person? bestMatch = null;
                int bestScore = -1;

                foreach (var sender in _userStore.GetAll())
                {
                    if (receiver.Username == sender.Username) continue;

                    if (receiver.BlockedUsernames.Contains(sender.Username)) continue;

                    int score = 0;

                    if (receiver.City == sender.City)
                    {
                        score += 30;
                    }

                    if (Math.Abs(receiver.Age - sender.Age) <= 2)
                    {
                        score += 20;
                    }

                    score += RandomNumberGenerator.GetInt32(0, 101);

                    if (score > bestScore) {
                        bestScore = score;
                        bestMatch = sender;
                    } 
                }

                if (bestMatch == null) continue;

                int randomNum = RandomNumberGenerator.GetInt32(0, 3);

                string randomMessage = messages[randomNum];

                string phone = randomMessage == "Nisam zainteresovan/a za upoznavanje."
                    ? ""
                    : bestMatch.PhoneNumber;

                receiver.WaitingConfirmation = true;

                await _hubContext.Clients.Client(receiver.ConnectionId)
                    .SendAsync("ReceiveLetter", bestMatch.Username, bestMatch.City, bestMatch.Age, phone, randomMessage);
            }

        }
    }
}
