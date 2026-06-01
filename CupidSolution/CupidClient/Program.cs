using Microsoft.AspNetCore.SignalR.Client;

namespace CupidClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Unesite username: ");
            string username = Console.ReadLine() ?? "";

            while (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username ne moze biti prazan, pokusajte ponovo: ");
                username = Console.ReadLine() ?? "";
            }

            Console.WriteLine("Unesite grad: ");
            string city = Console.ReadLine() ?? "";

            while (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("Grad ne moze biti prazan, pokusajte ponovo: ");
                city = Console.ReadLine() ?? "";
            }

            Console.WriteLine("Unesite godine: ");
            int age;

            while(!int.TryParse(Console.ReadLine(), out age) || age < 1)
            {
                Console.WriteLine("Godine moraju biti pozitivan broj, pokusajte ponovo: ");
            }

            Console.WriteLine("Unesite broj telefona (samo cifre): ");
            string phoneNumber = Console.ReadLine() ?? "";

            while (string.IsNullOrWhiteSpace(phoneNumber) || !phoneNumber.All(char.IsDigit))
            {
                Console.WriteLine("Broj telefona mora sadrzati samo cifre, pokusajte ponovo: ");
                phoneNumber = Console.ReadLine() ?? "";
            }

            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7117/cupidHub")
                .Build();

            connection.On<string>("RegistrationError", (errorMessage) =>
            {
                Console.WriteLine($"[GRESKA] {errorMessage}");
                Console.WriteLine("Pritisnite bilo koji taster za izlazak iz programa...");
                Console.ReadKey();
                Environment.Exit(0);
            });

            connection.On<string>("RegistrationSuccessed", (msg) =>
            {
                Console.WriteLine($"{msg}");
            });

            connection.On<string, string, int, string, string>("ReceiveLetter", async (username, city, age, phoneNumber, message) =>
            {
                Console.WriteLine();
                Console.WriteLine("=== NOVO PISMO ===");
                Console.WriteLine($"  Od:      {username}");
                Console.WriteLine($"  Grad:    {city}");
                Console.WriteLine($"  Godine:  {age}");
                if (!string.IsNullOrEmpty(phoneNumber))
                    Console.WriteLine($"  Telefon: {phoneNumber}");
                Console.WriteLine($"  Poruka:  {message}");
                Console.WriteLine("==================");
                Console.WriteLine("Pritisnite Enter za potvrdu prijema...");

                Console.ReadLine();
                await connection.InvokeAsync("ConfirmReceived");
                Console.WriteLine("[OK] Prijem potvrdjen.");
            });

            connection.On<string>("BlockError", (errorMessage) =>
            {
                Console.WriteLine($"[GRESKA] {errorMessage}");
            });

            connection.On<string>("BlockSuccess", (msg) =>
            {
                Console.WriteLine($"[OK] {msg}");
            });

            await connection.StartAsync();

            await connection.InvokeAsync("InitSinglePerson", username, city, age, phoneNumber);

            Console.WriteLine($"[OK] Prijavljeni ste kao '{username}'. Cekate pisma...");
            Console.WriteLine("Komanda: /block <username>");

            while (true)
            {
                string input = Console.ReadLine() ?? "";
                if (input.StartsWith("/block "))
                {
                    string toBlock = input.Substring(7);
                    await connection.InvokeAsync("BlockUser", toBlock);
                }
            }
        }
    }
}
