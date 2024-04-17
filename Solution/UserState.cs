using LibraryData;

namespace Solution;

public enum FileType
{
    Csv = 0,
    Json
}

public sealed class DialogState
{
    public FileType FileType { get; private set; } = FileType.Csv;

    public FieldSelection Selection { get; private set; } = FieldSelection.NONE;

    public void ChangeFileType(FileType type)
    {
        FileType = type;
    }

    public void ChangeFieldSelection(FieldSelection selection)
    {
        Selection = selection;
    }

    public string GetFileExtension()
    {
        return FileType == FileType.Csv ? ".csv" : ".json";
    }
}

public enum DataStateEnum
{
    None,
    Filtered,
    Sorted
}

/// <summary>
/// Класс UserState представляет собой состояние пользователя в системе.  
///Содержит информацию о загрузке данных, возможности загрузки файла,
///состоянии данных и обработанных данных, а также состоянии диалога.
/// </summary>
public class UserState
{
    public bool IsDataLoaded { get; private set; }
    
    public bool AllowUploadFile { get; private set; }

    public DataStateEnum DataState { get; private set; } = DataStateEnum.None;

    public List<CsvObj> Data { get; private set; } = new();

    public List<CsvObj> HandledData { get; private set; } = new();

    public DialogState DialogState { get; } = new();

    public void SetAllowUploadFile(bool isAllow)
    {
        AllowUploadFile = isAllow;
    }
    
    public void UpdateData(List<CsvObj> data)
    {
        IsDataLoaded = true;
        Data = data;
        DataState = DataStateEnum.None;
        HandledData.Clear();
    }

    public void ResetData()
    {
        IsDataLoaded = false;
        Data.Clear();
        HandledData.Clear();
        DataState = DataStateEnum.None;
    }

    public void SetSortedData(List<CsvObj> data)
    {
        HandledData.Clear();
        HandledData = data;
        DataState = DataStateEnum.Sorted;
    }

    public void SetSelectedData(List<CsvObj> data)
    {
        HandledData.Clear();
        HandledData = data;
        DataState = DataStateEnum.Filtered;
    }
}