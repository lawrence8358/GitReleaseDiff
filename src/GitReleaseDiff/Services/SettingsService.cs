using System.Security.Cryptography;
using System.Text;
using GitReleaseDiff.Models;
using Newtonsoft.Json;

namespace GitReleaseDiff.Services;

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
        var appFolder = Path.Combine(appDataPath, "GitReleaseDiff");

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
    public AppSettings Load()
    {
        if (File.Exists(_settingsFilePath))
        {
            var json = File.ReadAllText(_settingsFilePath);
            var settings = JsonConvert.DeserializeObject<AppSettings>(json);

            if (settings != null && !string.IsNullOrEmpty(settings.PersonalAccessToken))
            {
                try
                {
                    var encryptedData = Convert.FromBase64String(settings.PersonalAccessToken);
                    var decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
                    settings.PersonalAccessToken = Encoding.UTF8.GetString(decryptedData);
                }
                catch
                {
                    // 解密失敗可能是因為：
                    // 1. 舊版未加密的設定
                    // 2. Base64 格式錯誤
                    // 3. 不同的使用者或機器
                    // 在這些情況下，我們假設它是明文或無效，保持原樣
                }
            }

            return settings ?? new AppSettings();
        }

        return new AppSettings();
    }

    /// <summary>
    /// 儲存應用程式設定
    /// </summary>
    /// <param name="settings">要儲存的設定</param>
    public void Save(AppSettings settings)
    {
        try
        {
            // 建立深層複製以避免修改原始物件
            var settingsToSave = JsonConvert.DeserializeObject<AppSettings>(JsonConvert.SerializeObject(settings));

            if (settingsToSave != null && !string.IsNullOrEmpty(settingsToSave.PersonalAccessToken))
            {
                try
                {
                    var data = Encoding.UTF8.GetBytes(settingsToSave.PersonalAccessToken);
                    var encryptedData = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
                    settingsToSave.PersonalAccessToken = Convert.ToBase64String(encryptedData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加密失敗: {ex.Message}");
                    // 加密失敗時保留原始值（雖不安全但確保功能可用）
                }
            }

            var json = JsonConvert.SerializeObject(settingsToSave, Formatting.Indented);
            File.WriteAllText(_settingsFilePath, json);
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存設定失敗: {ex.Message}", ex);
        }
    }
}
