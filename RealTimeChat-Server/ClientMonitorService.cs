using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ClientMonitorService : BackgroundService
{
    private readonly ClientManager _clientManager;
    private readonly ILogger<ClientMonitorService> _logger;

    public ClientMonitorService(ClientManager clientManager, ILogger<ClientMonitorService> logger)
    {
        _clientManager = clientManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            MonitorClients();
            await Task.Delay(10000, stoppingToken); // Check every 10 seconds
        }
    }

    private void MonitorClients()
    {
        var inactiveClients = _clientManager.GetInactiveClients(TimeSpan.FromSeconds(30));
        foreach (var client in inactiveClients)
        {
            _logger.LogInformation($"Client {client.ClientId} is inactive and will be disconnected.");
            _clientManager.RemoveClient(client.ClientId);
        }
    }
}
