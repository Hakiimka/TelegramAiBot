namespace TelegramAiBot.Models;

public partial class MessageSequence
{

    public int Id { get; set; }

    public long UserId { get; set; }

    public string MessageText { get; set; } = null!;
}
