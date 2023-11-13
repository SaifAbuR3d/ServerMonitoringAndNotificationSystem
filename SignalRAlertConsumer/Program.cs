using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace SignalRAlertConsumer;

class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var connection = ConnectToSignalRServer(configuration);


        connection.On("ReceiveAlert", (string alert) => { Console.WriteLine(alert); });


        Console.ReadLine();
    }

    private static HubConnection ConnectToSignalRServer(IConfigurationRoot configuration)
    {
        string signalRHubUrl = configuration.GetConnectionString("SignalRHubUrl");

        var connection = new HubConnectionBuilder()
            .WithUrl(signalRHubUrl)
            .Build();

        try
        {
            connection.StartAsync().Wait();
            Console.WriteLine("SignalR connected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignalR connection error: {ex.Message}");
        }

        return connection;
    }
}