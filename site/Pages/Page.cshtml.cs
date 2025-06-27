using Microsoft.AspNetCore.SignalR.Client;

namespace site.Pages;


public class PageModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
{
    private static HubConnection _hubConnection;
    static PageModel()
    {
        // This page is used by Akamai GTM for health check purposes only. 
        // Here we use connection to the hub to cause the error in the site
        // by setting the environment variable as empty string 
        var hub = Environment.GetEnvironmentVariable("HUB"); 
        _hubConnection = new HubConnectionBuilder()
                .WithUrl(hub!)
                .WithAutomaticReconnect()
                .Build();

        _hubConnection.StartAsync().Wait();
    }
    public void OnGet()
    {
    }
}

