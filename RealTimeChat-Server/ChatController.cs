using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ClientManager _clientManager;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ClientManager clientManager, ILogger<ChatController> logger)
    {
        _clientManager = clientManager;
        _logger = logger;
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("API is working!");
    }

    [HttpGet("list")]
    public IActionResult List(int clientId)
    { 
        _clientManager.UpdateLastActivity(clientId);
        var client_list = _clientManager.GetAllClients();
        return Ok(new {client_list});
    }

    [HttpPost("connect")]
    public IActionResult Connect(string nome)
    {
        var clientHandler = _clientManager.AddClient(nome);
        _logger.LogInformation($"Client {clientHandler.ClientId} ({clientHandler.ClientName}) connected.");
        
        return Ok(new 
        { 
            clientId = clientHandler.ClientId, 
            clientName = clientHandler.ClientName, 
            message = "Client connected successfully." 
        });
    }


    [HttpPost("start-conversation")]
    public IActionResult StartConversation(string clientId, string targetClientId)
    {
        var client = _clientManager.GetClient(Convert.ToInt32(clientId));
        if (client == null) return NotFound("Client not found.");
        
        client.CurrentConversationWith = Convert.ToInt32(targetClientId);
        return Ok("Conversation started.");
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] Message message)
    {
        var sender = _clientManager.GetClient(message.SenderId);
        if (sender == null)
            return NotFound("Sender not connected.");

        var recipient = _clientManager.GetClient(message.ReceiverId);
        if (recipient == null)
            return NotFound("Recipient not connected.");

        await StoreMessageInConversation(sender, message.Content, message.ReceiverId);
        _logger.LogInformation($"Message from Client {message.SenderId} to Client {message.ReceiverId}: {message.Content}");

        return Ok(new { message = "Message sent and stored." });
    }

    [HttpGet("load-messages")]
    public async Task<IActionResult> LoadPreviousMessages(int clientId, int targetClientId)
    {
        var messages = await LoadMessagesFromDatabase(clientId, targetClientId);
        return Ok(new { messages });
    }

    private static async Task<List<Message>> LoadMessagesFromDatabase(int clientId, int targetClientId)
    {
        var messages = new List<Message>();
        using var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=banco123;Database=realtimechatdb");
        await conn.OpenAsync();

        string query = "SELECT client_id_1, client_id_2, message, timestamp FROM chat.conversations " +
                       "WHERE (client_id_1 = @client1 AND client_id_2 = @client2) " +
                       "OR (client_id_1 = @client2 AND client_id_2 = @client1) ORDER BY timestamp";

        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("client1", clientId);
        cmd.Parameters.AddWithValue("client2", targetClientId);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            messages.Add(new Message
            {
                SenderId = reader.GetInt32(0),
                ReceiverId = reader.GetInt32(1),
                Content = reader.GetString(2),
                Timestamp = reader.GetDateTime(3).ToString("o"),
                ConversationId = targetClientId
            });
        }
        return messages;
    }

    private static async Task StoreMessageInConversation(ClientHandler sender, string messageContent, int recipientId)
    {
        using var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=banco123;Database=realtimechatdb");
        await conn.OpenAsync();

        var query = "INSERT INTO chat.conversations (client_id_1, client_id_2, message, timestamp) " +
                    "VALUES (@client1, @client2, @message, @timestamp)";
        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("client1", sender.ClientId);
        cmd.Parameters.AddWithValue("client2", recipientId);
        cmd.Parameters.AddWithValue("message", messageContent);
        cmd.Parameters.AddWithValue("timestamp", DateTime.UtcNow);
        await cmd.ExecuteNonQueryAsync();
    }
}
