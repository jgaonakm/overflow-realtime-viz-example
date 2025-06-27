using Microsoft.AspNetCore.SignalR.Client;

namespace site.Pages;

public class IndexModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private static HubConnection _hubConnection;
    private static string site;

    static IndexModel()
    {
        // Used for identifying the site where the app is running and calculate the request # in the charts
        site = Environment.GetEnvironmentVariable("SITE")!;
        // SignalR hub location where we notify a request was received
        var hub = Environment.GetEnvironmentVariable("HUB");
        _hubConnection = new HubConnectionBuilder()
                .WithUrl(hub!)
                .WithAutomaticReconnect()
                .Build();

        _hubConnection.StartAsync().Wait();
    }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async void OnGet()
    {
        // Send the site name to the page for easy visualization. 
        ViewData["site"] = site;
        // Notify the SignalR Hub we received a request
        await _hubConnection.InvokeAsync("BroadcastRequestCount", site);
    }
}
