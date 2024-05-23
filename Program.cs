using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    private static ITelegramBotClient _botClient;
    
    private static ReceiverOptions _receiverOptions;
    
    static async Task Main()
    {
        
        _botClient = new TelegramBotClient("6832174326:AAGujEiZoFZt7rY4c2NoX7rxhzdN-Neh1pc"); 
        _receiverOptions = new ReceiverOptions 
        {
            AllowedUpdates = new[] 
            {
                UpdateType.Message, 
            },
         
            ThrowPendingUpdates = true, 
        };
        
        using var cts = new CancellationTokenSource();

        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота
        
        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"{me.FirstName} запущен!");
        
        await Task.Delay(-1); 
    }
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    var message = update.Message;
                    
                    var user = message.From;
                    
                    Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                    var chat = message.Chat;

                    if(chat.Id != -1002190818059){
                 
                    var forwardMessage = await botClient.ForwardMessageAsync( chatId: -1002190818059,
                    fromChatId: message.Chat.Id,
                    messageId: message.MessageId);
                    }else if(message.ReplyToMessage != null && chat.Id == -1002190818059){
                        var userId = message.ReplyToMessage.ForwardFrom;
                        var replyText = message.Text;
                        await botClient.SendTextMessageAsync(
                                    chatId: userId.Id,
                                    text: replyText
                                );

                    }
                    
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
  }