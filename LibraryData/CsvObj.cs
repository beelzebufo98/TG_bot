using System.Text.Json;
using System.Text.Json.Serialization;
using CsvHelper;

namespace LibraryData;

public sealed class CsvObj
{
    public CsvObj(CsvReader csvReader)
    {
        ID = csvReader.GetField<int>("ID");
        CulturalCenterName = csvReader.GetField<string>("CulturalCenterName");
        AdmArea = csvReader.GetField<string>("AdmArea");
        District = csvReader.GetField<string>("District");
        Address = csvReader.GetField<string>("Address");
        NumberOfAccessPoints = csvReader.GetField<int>("NumberOfAccessPoints");
        WiFiName = csvReader.GetField<string>("WiFiName");
        CoverageArea = csvReader.GetField<int>("CoverageArea");
        FunctionFlag = csvReader.GetField<string>("FunctionFlag");
        AccessFlag = csvReader.GetField<string>("AccessFlag");
        Password = csvReader.GetField<string>("Password");
        LatitudeWgs84 = csvReader.GetField<double>("Latitude_WGS84");
        LongitudeWgs84 = csvReader.GetField<double>("Longitude_WGS84");
        GlobalId = csvReader.GetField<string>("global_id");
        GeodataCenter = csvReader.GetField<string>("geodata_center");
        Geoarea = csvReader.GetField<string>("geoarea");
    }

    public CsvObj(JsonElement element)
    {
        ID = element.GetProperty("ID").GetInt32();
        CulturalCenterName = element.GetProperty("CulturalCenterName").GetString() ?? "";
        AdmArea = element.GetProperty("AdmArea").GetString() ?? "";
        District = element.GetProperty("District").GetString() ?? "";
        Address = element.GetProperty("Address").GetString() ?? "";
        NumberOfAccessPoints = element.GetProperty("NumberOfAccessPoints").GetInt32();
        WiFiName = element.GetProperty("WiFiName").GetString() ?? "";
        CoverageArea = element.GetProperty("CoverageArea").GetInt32();
        FunctionFlag = element.GetProperty("FunctionFlag").GetString() ?? "";
        AccessFlag = element.GetProperty("AccessFlag").GetString() ?? "";
        Password = element.GetProperty("Password").GetString() ?? "";
        LatitudeWgs84 = element.GetProperty("Latitude_WGS84").GetDouble();
        LongitudeWgs84 = element.GetProperty("Longitude_WGS84").GetDouble();
        GlobalId = element.GetProperty("global_id").GetString() ?? "";
        GeodataCenter = element.GetProperty("geodata_center").GetString() ?? "";
        Geoarea = element.GetProperty("geoarea").GetString() ?? "";
    }

    public int ID { get; init; }

    public string CulturalCenterName { get; init; }

    public string AdmArea { get; init; }

    public string District { get; init; }

    public string Address { get; init; }

    public int NumberOfAccessPoints { get; init; }

    public string WiFiName { get; init; }

    public int CoverageArea { get; init; }

    public string FunctionFlag { get; init; }

    public string AccessFlag { get; init; }

    public string Password { get; init; }

    [JsonPropertyName("Latitude_WGS84")]
    public double LatitudeWgs84 { get; init; }

    [JsonPropertyName("Longitude_WGS84")]
    public double LongitudeWgs84 { get; init; }

    [JsonPropertyName("global_id")] public string GlobalId { get; init; }

    [JsonPropertyName("geodata_center")] public string GeodataCenter { get; init; }

    [JsonPropertyName("geoarea")] public string Geoarea { get; init; }
}