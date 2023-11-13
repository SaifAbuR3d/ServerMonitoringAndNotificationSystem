using Microsoft.AspNetCore.SignalR.Client;

namespace MessageProcessingService;

public class SignalRClient
{
    private readonly HubConnection _hubConnection;

    public SignalRClient(string hubUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();
    }

    public void Start()
    {
        try
        {
            _hubConnection.StartAsync().Wait();
            Console.WriteLine("SignalR connected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignalR connection error: {ex.Message}");
        }
    }

    public async void SendAlert(Alert alert)
    {
        await _hubConnection.InvokeCoreAsync("SendAlert", args: new[] { alert.ToString() });
    }
}