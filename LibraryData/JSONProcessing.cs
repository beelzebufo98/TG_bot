using System.Text.Encodings.Web;
using System.Text.Json;

namespace LibraryData;

/// <summary>
/// Класс JSONProcessing обладает следующим функционалом
/// Пишет данные списка CsvObj в формат JSON и возвращает поток с данными.
/// Читает данные из потока и конвертирует JSON в список объектов типа CsvObj.
/// </summary>
public sealed class JSONProcessing
{
    public Stream Write(List<CsvObj> list)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var jsonString = JsonSerializer.Serialize(list, options);
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);
        writer.Write(jsonString);
        writer.Flush();
        memoryStream.Position = 0;
        return memoryStream;
    }


    public List<CsvObj> Read(Stream stream)
    {
        // Ensure the stream is at the beginning
        stream.Position = 0;

        // Parse the JSON content directly from the stream
        using var jsonDocument = JsonDocument.Parse(stream);

        var records = new List<CsvObj>();

        foreach (var element in jsonDocument.RootElement.EnumerateArray())
        {
            var record = new CsvObj(element);
            records.Add(record);
        }

        return records;
    }
}