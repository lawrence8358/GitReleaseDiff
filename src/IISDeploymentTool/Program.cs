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
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}