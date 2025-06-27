using Microsoft.AspNetCore.SignalR;

public class RequestHub : Hub
{
    // This is where we receive the calls from the SignalR 
    // clients deployed within each site
    public async Task BroadcastRequestCount(string count)
    {
        await Clients.All.SendAsync("UpdateRequestCount", count);
    }
}
