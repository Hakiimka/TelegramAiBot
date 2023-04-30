namespace TelegramAiBot.Models;

public partial class User
{
    public int Id { get; set; }

    public long UserId { get; set; }

    public string? FirstName { get; set; }

    public string? Username { get; set; }

    public string? LanguageCode { get; set; }
}
