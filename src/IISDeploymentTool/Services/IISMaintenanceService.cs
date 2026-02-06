using System.Text;

namespace IISDeploymentTool.Services;

/// <summary>
/// IIS 維護模式服務，負責管理 app_offline.htm 檔案
/// </summary>
public class IISMaintenanceService
{
    private const string APP_OFFLINE_FILENAME = "app_offline.htm";

    /// <summary>
    /// 啟用維護模式（創建 app_offline.htm）
    /// </summary>
    /// <param name="iisFolder">IIS 站台資料夾路徑</param>
    public void Enable(string iisFolder)
    {
        var filePath = Path.Combine(iisFolder, APP_OFFLINE_FILENAME);

        // 寫入簡單的維護頁面 HTML
        var content = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>系統維護中</title>
    <style>
        body {
            font-family: 'Microsoft JhengHei', Arial, sans-serif;
            text-align: center;
            padding: 50px;
            background-color: #f5f5f5;
        }
        .container {
            background-color: white;
            border-radius: 8px;
            padding: 40px;
            max-width: 600px;
            margin: 0 auto;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        h1 {
            color: #ff6b35;
            margin-bottom: 20px;
        }
        p {
            color: #666;
            font-size: 18px;
            line-height: 1.6;
        }
        .icon {
            font-size: 48px;
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""icon"">🔧</div>
        <h1>系統維護中</h1>
        <p>網站正在進行系統更新，請稍後再試。</p>
        <p>預計維護時間：3-5 分鐘</p>
    </div>
</body>
</html>";

        File.WriteAllText(filePath, content, Encoding.UTF8);

        // 等待 IIS 偵測到檔案（通常幾秒內）
        Thread.Sleep(2000);
    }

    /// <summary>
    /// 停用維護模式（移除 app_offline.htm）
    /// </summary>
    /// <param name="iisFolder">IIS 站台資料夾路徑</param>
    public void Disable(string iisFolder)
    {
        var filePath = Path.Combine(iisFolder, APP_OFFLINE_FILENAME);

        if (File.Exists(filePath))
        {
            // 嘗試刪除，如果被鎖定則重試
            int retryCount = 0;
            const int maxRetries = 5;

            while (retryCount < maxRetries)
            {
                try
                {
                    File.Delete(filePath);
                    return;
                }
                catch (IOException)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        throw new IOException($"無法刪除 {APP_OFFLINE_FILENAME}，檔案可能被鎖定。已重試 {maxRetries} 次。");
                    }
                    Thread.Sleep(1000);
                }
            }
        }
    }

    /// <summary>
    /// 檢查是否已在維護模式
    /// </summary>
    /// <param name="iisFolder">IIS 站台資料夾路徑</param>
    /// <returns>是否在維護模式</returns>
    public bool IsInMaintenanceMode(string iisFolder)
    {
        var filePath = Path.Combine(iisFolder, APP_OFFLINE_FILENAME);
        return File.Exists(filePath);
    }
}
