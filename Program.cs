using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("6656038988:AAERwyzOp-s3yoEra1hygwgy3NKRktmc8R8");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();


// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    #region Tugmalar
    var contactButton = new ReplyKeyboardMarkup(new[]
    {
       KeyboardButton.WithRequestContact("Kontakt yuborish📞")
    }
    )
    {
        ResizeKeyboard=true
    };
    #endregion

    if (messageText=="/start")
    {
        Message msg = await botClient.SendTextMessageAsync
            (
                chatId:chatId,
                text:$"🙋‍♂️Assalom alaykum <b>{message.From.FirstName}</b> botimizga xush kelibsiz\nBotimizga kontaktingizni yuboring",
                parseMode:ParseMode.Html,
                replyMarkup:contactButton,
                cancellationToken:cancellationToken
                
            );
    }

}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}