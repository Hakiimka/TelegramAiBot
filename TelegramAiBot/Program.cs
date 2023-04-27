using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


var config = configuration.Build();

var bot = new TelegramBotClient(config.GetSection("Bot_API_KEY").Value);

bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions() { AllowedUpdates = { } },
                new CancellationTokenSource().Token
            );

Console.ReadLine();

static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    try
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
        var message = update.Message;

        switch (update.Type)
        {
            case UpdateType.Message:
                switch (message.Text)
                {
                    case "/start":
                        //await Commands.Start(update, db, botClient);
                        break;

                    case "/clear":
                        //await Commands.ClearMessagesSequence(update, db, botClient);
                        break;

                    case "/clearlast":
                        //await Commands.clearLast(update, db, botClient);
                        break;

                    case "/users":
                        //await Commands.getUsers(update, db, botClient);
                        break;

                    case string a when a.Contains("/send"):
                        //await Commands.sendMessage(update, db, botClient);
                        break;
                    /*
                                                case string a when a.Contains("/imagine"):
                                                    await Commands.GenerateImage(update, db, botClient);
                                                    break;*/

                    default: break;//await Commands.Default(update, db, botClient); break;
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
    // Некоторые действия
    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
}
