var handler = new SocketsHttpHandler
{
    // Ensure the DNS config is refreshed continuously
    // https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines#dns-behavior
    PooledConnectionLifetime = TimeSpan.FromSeconds(15)

};
HttpClient httpClient = new HttpClient(handler);



Random random = new Random();

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("Stopping...");
    e.Cancel = true;
    cts.Cancel();
};

Console.Clear();
Console.WriteLine("Starting requests. Press Ctrl+C to stop.");
await StartRequestLoopAsync(cts.Token);


async Task StartRequestLoopAsync(CancellationToken cancellationToken)
{

    DateTime? firstFail = null;
    DateTime? lastFail = null;
    var position = Console.GetCursorPosition();
    int lineSize = 48;
    Console.SetCursorPosition(position.Left, position.Top + 4);
    Console.Write("".PadRight(lineSize, '-'));
    Console.SetCursorPosition(position.Left, position.Top + 6);
    Console.Write("".PadRight(lineSize, '-'));

    while (!cancellationToken.IsCancellationRequested)
    {
        _ = Task.Run(async () =>
        {
            // Property on Akamai GTM used to determine the IP of the backend site to send the traffic
            // based on selected configuration (failover, load balance, etc.)
            // https://techdocs.akamai.com/gtm/docs/how-it-works
            string url = "[property]"; 
            string path = "/";
            try
            {
                Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(position.Left, position.Top + 5);
                Console.Write("GET {0}".PadRight(lineSize), path);

                HttpResponseMessage response = await httpClient.GetAsync(url + path);
                if (response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (firstFail != null)
                    {
                        TimeSpan ttr = lastFail!.Value - firstFail.Value;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.SetCursorPosition(position.Left, position.Top + 10);
                        Console.WriteLine("TTR {0} s.", Math.Round(ttr.TotalSeconds, 2));
                    }
                }
                else
                {
                    if (firstFail == null)
                    {
                        firstFail = DateTime.Now;
                    }
                    lastFail = DateTime.Now;
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.ForegroundColor = response.IsSuccessStatusCode ? ConsoleColor.Green : ConsoleColor.Red;
                Console.SetCursorPosition(position.Left, position.Top + 5);
                Console.Write($"[{DateTime.Now:HH:mm:ss.fff}] - Status: {response.StatusCode}".PadRight(lineSize));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling {url}: {ex.Message}");
            }
        });

        await Task.Delay(500, cancellationToken);
    }
}
