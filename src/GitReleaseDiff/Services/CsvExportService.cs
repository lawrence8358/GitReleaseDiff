using GitReleaseDiff.Models;
using System.Text;

namespace GitReleaseDiff.Services;

/// <summary>
/// CSV 匯出服務類別，負責將檔案差異清單匯出為 CSV 格式
/// </summary>
public class CsvExportService
{
    /// <summary>
    /// 將檔案差異清單匯出為 CSV 檔案
    /// </summary>
    /// <param name="fileDiffs">檔案差異清單</param>
    /// <param name="filePath">匯出的檔案路徑</param>
    /// <param name="includeHeader">是否包含標題列</param>
    public void Export(List<FileDiffInfo> fileDiffs, string filePath, bool includeHeader = true)
    {
        var content = GenerateCsvContent(fileDiffs, includeHeader);
        // 使用 UTF-8 with BOM 以確保 Excel 正確識別編碼
        File.WriteAllText(filePath, content, new UTF8Encoding(true));
    }

    /// <summary>
    /// 產生 CSV 內容字串
    /// </summary>
    /// <param name="fileDiffs">檔案差異清單</param>
    /// <param name="includeHeader">是否包含標題列</param>
    /// <returns>CSV 格式的字串內容</returns>
    public string GenerateCsvContent(List<FileDiffInfo> fileDiffs, bool includeHeader = true)
    {
        var sb = new StringBuilder();

        // 加入標題列
        if (includeHeader)
        {
            sb.AppendLine("資料夾路徑,檔案名稱,附檔名,狀態,完整路徑,舊檔案路徑");
        }

        // 加入資料列
        foreach (var diff in fileDiffs)
        {
            sb.AppendLine(string.Join(",",
                EscapeCsvField(diff.FolderPath),
                EscapeCsvField(diff.FileName),
                EscapeCsvField(diff.FileExtension),
                EscapeCsvField(diff.StatusDescription),
                EscapeCsvField(diff.FilePath),
                EscapeCsvField(diff.OldFilePath ?? string.Empty)
            ));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 轉義 CSV 欄位中的特殊字元
    /// </summary>
    /// <param name="field">欄位值</param>
    /// <returns>轉義後的欄位值</returns>
    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return string.Empty;
        }

        // 如果包含逗號、引號或換行，需要用引號包住並轉義內部引號
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}
