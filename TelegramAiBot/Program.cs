using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramAiBot.ChatCommands;
using TelegramAiBot.Models.DBContext;

var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var config = configuration.Build();

var dbContext = new AiTelegramBotDbContext(config.GetSection("AiDBConnectionString").Value);

var bot = new TelegramBotClient(config.GetSection("Bot_API_KEY").Value);

bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions() { AllowedUpdates = { } },
                new CancellationTokenSource().Token
            );

Console.ReadLine();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    try
    {
        var message = update.Message;

        switch (update.Type)
        {
            case UpdateType.Message:
                switch (message.Text)
                {
                    case "/start":
                        await Commands.Start(update, botClient, dbContext);
                        break;

                    case "/clear":
                        await Commands.ClearMessagesSequence(update, botClient, dbContext);
                        break;

                    case "/clearlast":
                        await Commands.ClearLast(update, botClient, dbContext);
                        break;

                    case "/users":
                        await Commands.GetUsers(update, botClient, dbContext);
                        break;

                    case string a when a.Contains("/send"):
                        await Commands.SendMessage(update, botClient, dbContext);
                        break;

                    case string a when a.Contains("/imagine"):
                        await Commands.GenerateImage(update, botClient, dbContext);
                        break;

                    default: await Commands.Default(update, botClient, dbContext); break;
                }
                break;

        }

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        await botClient.SendTextMessageAsync(update.Message.Chat,
            "Произошла ошибка, попробуйте очистить всю историю сообщений /clear или удалить только последнее сообщение /clearlast ");
    }


}

static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
}
