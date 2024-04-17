using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace LibraryData;

public sealed class CSVProcessing
{
    public Stream CsvWrite(IEnumerable<CsvObj> list)
    {
        var memoryStream = new MemoryStream();

        using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, 1024, true))
        {
            writer.WriteLine(
                "ID;CulturalCenterName;AdmArea;District;Address;NumberOfAccessPoints;WiFiName;CoverageArea;FunctionFlag;AccessFlag;Password;Latitude_WGS84;Longitude_WGS84;global_id;geodata_center;geoarea");
            writer.WriteLine(
                "Код;Наименование культурного центра;Административный округ;Район;Адрес;Количество точек доступа;Имя Wi-Fi сети;Зона покрытия, в метрах;Признак функционирования;Условия доступа;Пароль;Широта в WGS-84;Долгота в WGS-84;global_id;geodata_center;geoarea");
            var csvWriter = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                Encoding = Encoding.UTF8,
                HasHeaderRecord = false
            });
            csvWriter.WriteRecords(list);
            writer.Flush();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public List<CsvObj> CsvRead(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true
        };
        var reader = new StreamReader(stream);
        var csvReader = new CsvReader(reader, config);
        var records = new List<CsvObj>();

        csvReader.Read();
        csvReader.ReadHeader();
        csvReader.Read();

        while (csvReader.Read())
        {
            var csvObj = new CsvObj(csvReader);
            records.Add(csvObj);
        }

        return records;
    }
}