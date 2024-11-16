public class ClientHandler
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; }
    public int? CurrentConversationWith { get; set; }

    public ClientHandler()
    {
        LastActivity = DateTime.Now;
    }
}
