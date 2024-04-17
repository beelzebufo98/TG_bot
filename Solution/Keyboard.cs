using Telegram.Bot.Types.ReplyMarkups;

namespace Solution;
/// <summary>
/// Класс для хранения клавиатур, необходимых для диалога с пользователем.
/// </summary>
public static class Keyboard
{
    public static InlineKeyboardMarkup GetStartKeys()
    {
        return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Загрузить данные в формате CSV", KeyboardCallbackType.LoadCsv)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Загрузить данные в формате JSON", KeyboardCallbackType.LoadJson)
            }
        });
    }

    public static InlineKeyboardMarkup GetDownloadKeys()
    {
        return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Скачать данные в формате CSV", KeyboardCallbackType.DownloadCsv)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Скачать данные в формате JSON",
                    KeyboardCallbackType.DownloadJson)
            }
        });
    }

    public static InlineKeyboardMarkup GetFileActionsKeys()
    {
        return new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅ Вернуться в главное меню", KeyboardCallbackType.MainMenu)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Отсортировать данные", KeyboardCallbackType.Sort)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Произвести выборку", KeyboardCallbackType.Filter)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Скачать данные в формате CSV", KeyboardCallbackType.DownloadCsv)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Скачать данные в формате JSON",
                    KeyboardCallbackType.DownloadJson)
            }
        });
    }

    public static InlineKeyboardMarkup GetFileContinueActionsKeys()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅ Вернуться в главное меню", KeyboardCallbackType.MainMenu)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅ Вернуться в меню работы с файлом",
                    KeyboardCallbackType.FileMenu)
            }
        });
    }

    public static InlineKeyboardMarkup GetFilterKeys()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Отфильтровать по CoverageArea",
                    KeyboardCallbackType.FilterByCoverageArea)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Отфильтровать по WiFiName",
                    KeyboardCallbackType.FilterByWiFiName)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Отфильтровать по District",
                    KeyboardCallbackType.FilterByDistrict)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Отфильтровать по AccessFlag",
                    KeyboardCallbackType.FilterByAccessFlag)
            }
        });
    }

    public static InlineKeyboardMarkup GetHandledDataKeys()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅ Вернуться в главное меню", KeyboardCallbackType.MainMenu)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Сохранить обработанные данные в CSV",
                    KeyboardCallbackType.DownloadHandledDataCsv)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Сохранить обработанные данные в JSON",
                    KeyboardCallbackType.DownloadHandledDataJson)
            }
        });
    }

    public static InlineKeyboardMarkup GetSortKeys()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Сортировать по CulturalCenterName",
                    KeyboardCallbackType.SortByName),
                InlineKeyboardButton.WithCallbackData("Сортировать по NumberOfAccessPoints",
                    KeyboardCallbackType.SortByAccessPoint)
            }
        });
    }
}