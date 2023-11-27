using Microsoft.AspNetCore.SignalR.Client;

namespace ChatAppServer
{
    public class Program
    {
        static async void Main(string[] args)
        {
            var hubConnection = new HubConnectionBuilder()
            .WithUrl("")
            .Build();

            hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                Console.WriteLine($"Received message: {message}");
            });

            await hubConnection.StartAsync();

            Console.Write("Enter a message:");
            var input = Console.ReadLine();

            await hubConnection.InvokeAsync("SendMessage", "Matze", input);

            Console.WriteLine("Press a key to exit");
            Console.ReadLine();
            await hubConnection.StopAsync();
        }
    }
}

