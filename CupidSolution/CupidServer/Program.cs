
using CupidServer.Hubs;
using CupidServer.Services;

namespace CupidServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<UserStore>();
            builder.Services.AddHostedService<CupidService>();

            var app = builder.Build();

            app.MapHub<CupidHub>("/cupidHub");

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
