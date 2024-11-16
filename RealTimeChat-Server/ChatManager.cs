using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

public class ClientManager
{
    private readonly ConcurrentDictionary<int, ClientHandler> _clients = new();
    private int _nextClientId = 1;

    public ClientHandler AddClient(string nome)
    {
        var clientHandler = new ClientHandler
        {
            ClientName = nome,
            ClientId = _nextClientId++,
            LastActivity = DateTime.Now
        };
        _clients.TryAdd(clientHandler.ClientId, clientHandler);
        return clientHandler;
    }

    public ClientHandler? GetClient(int clientId)
    {
        _clients.TryGetValue(clientId, out var client);
        return client;
    }

    public IEnumerable<ClientHandler> GetAllClients() => _clients.Values;

    public bool RemoveClient(int clientId)
    {
        return _clients.TryRemove(clientId, out _);
    }

    public void UpdateLastActivity(int clientId)
    {
        if (_clients.TryGetValue(clientId, out var client))
        {
            client.LastActivity = DateTime.Now;
        }
    }

    public IEnumerable<ClientHandler> GetInactiveClients(TimeSpan inactivityThreshold)
    {
        var now = DateTime.Now;
        return _clients.Values.Where(c => (now - c.LastActivity) > inactivityThreshold).ToList();
    }
}
