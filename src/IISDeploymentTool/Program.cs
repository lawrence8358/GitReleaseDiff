using System.Diagnostics;
using System.Security.Principal;

namespace IISDeploymentTool;

/// <summary>
/// 應用程式進入點
/// </summary>
static class Program
{
    /// <summary>
    /// 應用程式主要進入點
    /// </summary>
    [STAThread]
    static void Main()
    {
        // 檢查是否以系統管理員身分執行
        if (!IsRunAsAdministrator())
        {
            // 嘗試以系統管理員身分重新啟動
            if (!RestartAsAdministrator())
            {
                // 使用者拒絕提升權限或重新啟動失敗
                MessageBox.Show(
                    "本程式需要系統管理員權限才能操作 IIS 應用程式池。\n\n" +
                    "請以系統管理員身分執行本程式。",
                    "需要系統管理員權限",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            return;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }

    /// <summary>
    /// 檢查是否以系統管理員身分執行
    /// </summary>
    /// <returns>是否以系統管理員身分執行</returns>
    private static bool IsRunAsAdministrator()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 以系統管理員身分重新啟動應用程式
    /// </summary>
    /// <returns>是否成功重新啟動</returns>
    private static bool RestartAsAdministrator()
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = Environment.ProcessPath ?? Application.ExecutablePath,
                UseShellExecute = true,
                Verb = "runas" // 要求提升權限
            };

            Process.Start(processInfo);
            return true;
        }
        catch
        {
            // 使用者拒絕 UAC 提示或發生其他錯誤
            return false;
        }
    }
}