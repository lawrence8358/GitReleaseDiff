using IISDeploymentTool.Models;
using Newtonsoft.Json;

namespace IISDeploymentTool.Services;

/// <summary>
/// 設定服務類別，負責儲存和載入應用程式設定
/// </summary>
public class SettingsService
{
    private readonly string _settingsFilePath;

    /// <summary>
    /// 初始化設定服務
    /// </summary>
    public SettingsService()
    {
        // 設定檔案儲存在使用者的 AppData 資料夾
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "IISDeploymentTool");

        // 確保資料夾存在
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }

        _settingsFilePath = Path.Combine(appFolder, "settings.json");
    }

    /// <summary>
    /// 使用指定的設定檔案路徑初始化設定服務
    /// </summary>
    /// <param name="settingsFilePath">設定檔案路徑</param>
    public SettingsService(string settingsFilePath)
    {
        _settingsFilePath = settingsFilePath;
    }

    /// <summary>
    /// 載入應用程式設定
    /// </summary>
    /// <returns>應用程式設定</returns>
    public DeploymentSettings Load()
    {
        if (File.Exists(_settingsFilePath))
        {
            try
            {
                var json = File.ReadAllText(_settingsFilePath);
                var settings = JsonConvert.DeserializeObject<DeploymentSettings>(json);
                return settings ?? new DeploymentSettings();
            }
            catch
            {
                // 若讀取或解析失敗，回傳預設設定
                return new DeploymentSettings();
            }
        }

        return new DeploymentSettings();
    }

    /// <summary>
    /// 儲存應用程式設定
    /// </summary>
    /// <param name="settings">要儲存的設定</param>
    public void Save(DeploymentSettings settings)
    {
        try
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_settingsFilePath, json);
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存設定失敗: {ex.Message}", ex);
        }
    }
}
