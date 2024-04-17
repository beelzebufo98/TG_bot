using LibraryData;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Solution;

public class TelegramBot
{
    private static readonly UserStateManager UserManager = new();

    private readonly ITelegramBotClient _botClient =
        new TelegramBotClient("7000031145:AAHm5FGIeKJwEPmO6sLvVNB4QLkI8Kixbo4");

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var options = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery },
            ThrowPendingUpdates = true
        };
        var bot = await _botClient.GetMeAsync(cancellationToken);
        _botClient.StartReceiving(UpdateHandlerAsync, ErrorHandler, options, cancellationToken);
        Log.Information($"{bot.FirstName} запущен!");
        await Task.Delay(-1, cancellationToken);
    }

    private static async Task UpdateHandlerAsync(ITelegramBotClient client, Update update,
        CancellationToken token)
    {
        try
        {
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                var chatId = message?.Chat.Id;

                if (chatId is null) return;

                var user = UserManager[(long)chatId];

                switch (message?.Type)
                {
                    case MessageType.Text:
                    {
                        var text = message.Text;
                        if (text == "/start")
                        {
                            await MainMenuMessage(client, (long)chatId, token);
                            return;
                        }

                        if (!user.IsDataLoaded)
                        {
                            await DataNotLoadedMessage(client, (long)chatId, token);
                            await MainMenuMessage(client, (long)chatId, token);
                            return;
                        }

                        if (user.DialogState.Selection == FieldSelection.NONE)
                        {
                            await client.SendTextMessageAsync(chatId,
                                "Невозможно обработать текстовое сообщение",
                                replyMarkup: Keyboard.GetFileActionsKeys(),
                                cancellationToken: token);
                            return;
                        }

                        var filteredData =
                            DataProcessing.FilterRecords(user.Data, user.DialogState.Selection, text ?? "");
                        user.SetSelectedData(filteredData);
                        user.DialogState.ChangeFieldSelection(FieldSelection.NONE);
                        await client.SendTextMessageAsync(chatId,
                            $"Данные успешно отфильтрованны. Количество записей {filteredData.Count}",
                            replyMarkup: Keyboard.GetHandledDataKeys(),
                            cancellationToken: token);
                        return;
                    }
                    case MessageType.Document:
                    {
                        var fileId = update.Message?.Document?.FileId;
                        if (string.IsNullOrEmpty(fileId)) return;
                        var file = await client.GetFileAsync(fileId, token);
                        var fileExtension = Path.GetExtension(file.FilePath);
                        var fileType = user.DialogState.FileType;

                        if (!user.AllowUploadFile)
                        {
                            await client.SendTextMessageAsync(chatId,
                                "Невозможно обработать файл, выберите действие:",
                                replyMarkup: Keyboard.GetStartKeys(),
                                cancellationToken: token);
                            return;
                        }
                        
                        if (fileExtension is not (".json" or ".csv"))
                        {
                            await client.SendTextMessageAsync(chatId,
                                "Некорректное расширение файла",
                                cancellationToken: token);
                            return;
                        }

                        if ((fileType == FileType.Json && fileExtension == ".json") ||
                            (fileType == FileType.Csv && fileExtension == ".csv"))
                            try
                            {
                                await using Stream fileStream = new MemoryStream();
                                await client.DownloadFileAsync(file.FilePath ?? "", fileStream, token);
                                fileStream.Seek(0, SeekOrigin.Begin);
                                var objects = fileType == FileType.Csv
                                    ? new CSVProcessing().CsvRead(fileStream)
                                    : new JSONProcessing().Read(fileStream);
                                user.UpdateData(objects);
                                await client.SendTextMessageAsync(chatId,
                                    $"Данные успешно прочитаны! Количество объектов {objects.Count}",
                                    cancellationToken: token);
                                await client.SendTextMessageAsync(chatId,
                                    "Выберите действие с данными:",
                                    replyMarkup: Keyboard.GetFileActionsKeys(),
                                    cancellationToken: token);
                                user.SetAllowUploadFile(false);
                            }
                            catch (Exception)
                            {
                                await client.SendTextMessageAsync(chatId, "Не удалось прочитать данные файла!",
                                    cancellationToken: token);
                            }
                        else
                            await client.SendTextMessageAsync(chatId,
                                $"Некорректное расширение файла, ожидается файл в формате {user.DialogState.GetFileExtension()}",
                                cancellationToken: token);

                        return;
                    }
                    default:
                    {
                        return;
                    }
                }
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                var callbackQuery = update.CallbackQuery;
                if (callbackQuery == null)
                    return;
                var chatId = callbackQuery.Message?.Chat.Id ?? 0;
                var state = UserManager[chatId];

                Log.Information($"User with id {chatId} completed CallbackQuery action {callbackQuery.Data} ");

                switch (callbackQuery.Data)
                {
                    case KeyboardCallbackType.MainMenu:
                    {
                        state.ResetData();
                        await MainMenuMessage(client, chatId, token);
                        return;
                    }
                    case KeyboardCallbackType.FileMenu:
                    {
                        if (!state.IsDataLoaded)
                        {
                            await DataNotLoadedMessage(client, chatId, token);
                            await MainMenuMessage(client, chatId, token);
                            return;
                        }

                        await client.SendTextMessageAsync(chatId,
                            "Меню работы с файлом",
                            replyMarkup: Keyboard.GetFileActionsKeys(),
                            cancellationToken: token);
                        return;
                    }
                    case KeyboardCallbackType.LoadCsv:
                    {
                        state.DialogState.ChangeFileType(FileType.Csv);
                        await client.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: token);
                        await client.SendTextMessageAsync(chatId, "Загрузите CSV файл с данными",
                            cancellationToken: token);
                        state.SetAllowUploadFile(true);
                        return;
                    }
                    case KeyboardCallbackType.LoadJson:
                    {
                        state.DialogState.ChangeFileType(FileType.Json);
                        await client.AnswerCallbackQueryAsync(callbackQuery.Id,
                            cancellationToken: token);
                        await client.SendTextMessageAsync(
                            chatId,
                            "Загрузите JSON файл с данными",
                            cancellationToken: token);
                        state.SetAllowUploadFile(true);
                        return;
                    }
                    case KeyboardCallbackType.DownloadHandledDataCsv:
                    case KeyboardCallbackType.DownloadHandledDataJson:
                    {
                        if (state.DataState == DataStateEnum.None)
                        {
                            await client.SendTextMessageAsync(chatId,
                                "Нельзя сохранить необработанные данные",
                                replyMarkup: Keyboard.GetStartKeys(),
                                cancellationToken: token);
                            return;
                        }

                        var type = callbackQuery.Data == KeyboardCallbackType.DownloadHandledDataCsv
                            ? "CSV"
                            : "JSON";

                        await using var dataStream = type == "CSV"
                            ? new CSVProcessing().CsvWrite(state.HandledData)
                            : new JSONProcessing().Write(state.HandledData);

                        await client.SendDocumentAsync(chatId,
                            InputFile.FromStream(dataStream, $"data.{type.ToLower()}"),
                            caption: $"Данные сохранены в формате {type}",
                            cancellationToken: token);
                        await FileRepeatMenuMessage(client, chatId, token);
                        return;
                    }
                    case KeyboardCallbackType.DownloadJson:
                    case KeyboardCallbackType.DownloadCsv:
                    {
                        if (!state.IsDataLoaded)
                        {
                            await DataNotLoadedMessage(client, chatId, token);
                            await MainMenuMessage(client, chatId, token);
                            return;
                        }

                        var extension = callbackQuery.Data == KeyboardCallbackType.DownloadCsv
                            ? "CSV"
                            : "JSON";
                        await using var dataStream = extension == "CSV"
                            ? new CSVProcessing().CsvWrite(state.Data)
                            : new JSONProcessing().Write(state.Data);
                        dataStream.Seek(0, SeekOrigin.Begin);

                        await client.SendDocumentAsync(chatId,
                            InputFile.FromStream(dataStream, $"data.{extension.ToLower()}"),
                            caption: $"Данные сохранены в формате {extension}",
                            cancellationToken: token);
                        await FileRepeatMenuMessage(client, chatId, token);
                        return;
                    }
                    case KeyboardCallbackType.Sort:
                    {
                        if (!state.IsDataLoaded)
                        {
                            await DataNotLoadedMessage(client, chatId, token);
                            return;
                        }

                        await client.SendTextMessageAsync(chatId,
                            "Меню сортировки:",
                            replyMarkup: Keyboard.GetSortKeys(),
                            cancellationToken: token);
                        return;
                    }
                    case KeyboardCallbackType.SortByName:
                    case KeyboardCallbackType.SortByAccessPoint:
                    {
                        if (!state.IsDataLoaded)
                        {
                            await DataNotLoadedMessage(client, chatId, token);
                            return;
                        }

                        var sortType = callbackQuery.Data == KeyboardCallbackType.SortByName
                            ? DataProcessing.FieldSort.CULTURALCENTERNAME
                            : DataProcessing.FieldSort.NUMBEROFACCESSPOINTS;
                        var sortedData = DataProcessing.SortRecords(state.Data, sortType);
                        state.SetSortedData(sortedData);
                        await client.SendTextMessageAsync(chatId,
                            "Данные успешно отсортированны! \n Выберите дальнейшее действие:",
                            replyMarkup: Keyboard.GetHandledDataKeys(),
                            cancellationToken: token);
                        return;
                    }
                    case KeyboardCallbackType.Filter:
                    {
                        if (!state.IsDataLoaded)
                        {
                            await DataNotLoadedMessage(client, chatId, token);
                            return;
                        }

                        await client.SendTextMessageAsync(chatId,
                            "Меню фильтрации:",
                            replyMarkup: Keyboard.GetFilterKeys(),
                            cancellationToken: token);
                        return;
                    }
                    case KeyboardCallbackType.FilterByDistrict:
                    case KeyboardCallbackType.FilterByWiFiName:
                    case KeyboardCallbackType.FilterByAccessFlag:
                    case KeyboardCallbackType.FilterByCoverageArea:
                    {
                        if (!state.IsDataLoaded)
                        {
                            await DataNotLoadedMessage(client, chatId, token);
                            await MainMenuMessage(client, chatId, token);
                            return;
                        }

                        state.DialogState.ChangeFieldSelection(ConvertButtonToSelection(callbackQuery.Data));
                        await client.SendTextMessageAsync(chatId,
                            "Введите и отправьте значение для фильтрации",
                            cancellationToken: token);
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error,
        CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var errorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Log.Error(errorMessage);
        return Task.CompletedTask;
    }

    private static async Task DataNotLoadedMessage(ITelegramBotClient client,
        long chatId, CancellationToken token)
    {
        await client.SendTextMessageAsync(chatId,
            "Данные для обработки не были загружены!",
            cancellationToken: token);
    }

    private static async Task FileRepeatMenuMessage(ITelegramBotClient client,
        long chatId, CancellationToken token)
    {
        await client.SendTextMessageAsync(chatId,
            "Выберите дальнейшее действие",
            replyMarkup: Keyboard.GetFileContinueActionsKeys(),
            cancellationToken: token);
    }

    private static async Task MainMenuMessage(ITelegramBotClient client,
        long chatId, CancellationToken token)
    {
        await client.SendTextMessageAsync(chatId,
            "Главное меню",
            replyMarkup: Keyboard.GetStartKeys(),
            cancellationToken: token);
    }

    private static FieldSelection ConvertButtonToSelection(string button)
    {
        return button switch
        {
            KeyboardCallbackType.FilterByDistrict => FieldSelection.DISTRICT,
            KeyboardCallbackType.FilterByAccessFlag => FieldSelection.ACCESSFLAG,
            KeyboardCallbackType.FilterByCoverageArea => FieldSelection.COVERAGEAREA,
            KeyboardCallbackType.FilterByWiFiName => FieldSelection.WIFINAME,
            _ => FieldSelection.NONE
        };
    }
}