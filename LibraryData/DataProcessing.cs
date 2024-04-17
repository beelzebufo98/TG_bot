namespace LibraryData;

public enum FieldSelection
{
    NONE = 0,
    COVERAGEAREA = 1,
    WIFINAME = 2,
    DISTRICT = 3,
    ACCESSFLAG = 4
}

public static class DataProcessing
{

    public enum FieldSort
    {
        CULTURALCENTERNAME = 1,
        NUMBEROFACCESSPOINTS = 2
    }

    public static List<CsvObj> FilterRecords(List<CsvObj> records, FieldSelection field, string value)
    {
        switch (field)
        {
            case FieldSelection.COVERAGEAREA:
                return records.Where(record => record.CoverageArea.ToString() == value).ToList();
            case FieldSelection.WIFINAME:
                return records.Where(record => record.WiFiName == value).ToList();
            case FieldSelection.DISTRICT:
                return records.Where(record => record.District == value).ToList();
            case FieldSelection.ACCESSFLAG:
                return records.Where(record => record.AccessFlag == value).ToList();
            default:
                throw new ArgumentException("Invalid field selection.");
        }
    }

    public static List<CsvObj> SortRecords(List<CsvObj> records, FieldSort choice)
    {
        switch (choice)
        {
            case FieldSort.CULTURALCENTERNAME:

                return records.OrderBy(record => record.CulturalCenterName).ToList();
            case FieldSort.NUMBEROFACCESSPOINTS:

                return records.OrderBy(record => record.NumberOfAccessPoints).ToList();
            default:
                Console.WriteLine("Invalid choice.");
                return records;
        }
    }
}