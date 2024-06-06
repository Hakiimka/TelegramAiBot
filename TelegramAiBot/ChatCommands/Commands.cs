using Newtonsoft.Json;
using OpenAI_API.Chat;
using OpenAI_API.Images;
using OpenAI_API.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramAiBot.Models;
using TelegramAiBot.Models.DBContext;
#pragma warning disable CS8602 

namespace TelegramAiBot.ChatCommands
{
    internal class Commands
    {

        public static async Task Start(Update update, ITelegramBotClient botClient, AiTelegramBotDbContext dbContext)
        {
            var message = update.Message;


            var user = dbContext.Users.Where(user => user.UserId == message.Chat.Id).FirstOrDefault();


            if (user is null)
            {
                dbContext.Users.Add(new Models.User
                {
                    FirstName = update.Message.From.FirstName,
                    Username = update.Message.From.Username,
                    LanguageCode = update.Message.From.LanguageCode,
                    UserId = update.Message.Chat.Id,
                });
                dbContext.SaveChanges();

                await botClient.SendTextMessageAsync(message.Chat.Id, "Привет! Я отвечу на любой твой вопрос!"
                   );
                return;
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Я жду твой вопрос!"
                    );
                return;
            }
        }
        private static OpenAI_API.OpenAIAPI api = new OpenAI_API.OpenAIAPI(" ");

        public static async Task Default(Update update, ITelegramBotClient botClient, AiTelegramBotDbContext dbContext)
        {
            var user = update.Message.From;
            Console.WriteLine($"Пользователь {user.Username} {user.Id} сделал запрос: {update.Message.Text}");
            dbContext.MessageSequences.Add(new MessageSequence { UserId = update.Message.Chat.Id, MessageText = update.Message.Text });
            dbContext.SaveChanges();

            var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.9,
                MaxTokens = 3000,
                Messages = GetChatMessages(update, dbContext)
            }); ;
            Console.WriteLine(result);
            var message = update.Message;

            await botClient.SendTextMessageAsync(message.Chat.Id, result.ToString());

            return;
        }

        public static async Task ClearMessagesSequence(Update update, ITelegramBotClient botClient, AiTelegramBotDbContext dbContext)
        {
            var sequenses = dbContext.MessageSequences.Where(x => x.UserId == update.Message.Chat.Id);
            dbContext.MessageSequences.RemoveRange(sequenses);
            dbContext.SaveChanges();
            await botClient.SendTextMessageAsync(update.Message.Chat, "История успешно удалена");
            return;
        }

        public static async Task GenerateImage(Update update, ITelegramBotClient botClient, AiTelegramBotDbContext dbContext)
        {
            string prompt = update.Message.Text.Replace("/imagine", "");
            var user = update.Message.From;
            Console.WriteLine($"Пользователь {user.Username} {user.Id} сделал запрос: {prompt}");

            var result = await api.ImageGenerations.CreateImageAsync(new ImageGenerationRequest()
            {
                NumOfImages = 2,
                Prompt = prompt,
            });
            foreach (var pic in result.Data)
            {
                await botClient.SendPhotoAsync(update.Message.Chat, pic.Url);
            }

            return;
        }

        public static List<ChatMessage> GetChatMessages(Update update, AiTelegramBotDbContext dbContext)
        {
            var userMessages = dbContext.MessageSequences.Where(x => x.UserId == update.Message.Chat.Id).ToList();

            var resultMessages = new List<ChatMessage>();
            foreach (var message in userMessages)
            {
                resultMessages.Add(new ChatMessage(ChatMessageRole.User, message.MessageText));
            }

            return resultMessages;

        }

        public static async Task SendMessage(Update update, ITelegramBotClient botClient, AiTelegramBotDbContext dbContext)
        {
            var chatid = update.Message.Text.Split(' ')[1];
            var args = update.Message.Text.Replace("/send", "");
            args = args.Replace(update.Message.Chat.Id.ToString(), "");
            try
            {
                await botClient.SendTextMessageAsync(chatid, args);
            }
            catch (Exception ex) { await botClient.SendTextMessageAsync(update.Message.Chat, ex.Message); }

        }

        public static async Task GetUsers(Update update, ITelegramBotClient botClient, AiTelegramBotDbContext dbContext)
        {
            const int adminid = 556058839;

            if (adminid == update.Message.Chat.Id)
                await botClient.SendTextMessageAsync(update.Message.Chat, JsonConvert.SerializeObject(dbContext.Users.ToList()));
            else
                await botClient.SendTextMessageAsync(update.Message.Chat, "ты не админ");
        }

        public static async Task ClearLast(Update update, ITelegramBotClient botClient, AiTelegramBotDbContext dbContext)
        {
            var sequenses = dbContext.MessageSequences.Where(x => x.UserId == update.Message.Chat.Id).ToList();

            if (sequenses.Any())
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, $"успешно удалено сообщение {sequenses[sequenses.Count - 1].MessageText}");
                dbContext.MessageSequences.Remove(sequenses[sequenses.Count - 1]);
                dbContext.SaveChanges();

            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, "история пуста");
            }

        }
    }
}
