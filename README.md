# Git Release Diff - Git 版本比對工具

## 專案目的

此工具旨在協助開發人員在進行程式上版時，能夠快速列出「本次上版程式」與「上次上版程式」之間的檔案差異清單。透過輸入同一分支的兩個 Commit ID，即可自動查詢並顯示所有變更的檔案。

## 功能說明

### 主要功能

1. **版本差異比對**
   - 支援輸入 Azure DevOps / TFS 的 Git 儲存庫網址
   - 支援輸入 Commit ID（完整或短碼格式）
   - 自動查詢兩個 Commit 之間的所有檔案差異

2. **結果呈現**
   - 依資料夾路徑和檔案名稱排序顯示
   - 相同資料夾的檔案會集中顯示
   - 顯示檔案附檔名，方便依檔案類型排序
   - 以顏色區分檔案狀態：
     - 🟢 新增（淺綠色）
     - 🟡 修改（淺黃色）
     - 🔴 刪除（淺紅色）
     - 🔵 重新命名（淺藍色）

3. **CSV 匯出**
   - 支援將比對結果匯出為 CSV 格式
   - 包含欄位：資料夾路徑、檔案名稱、附檔名、狀態、完整路徑、舊檔案路徑

4. **設定記憶**
   - 自動記住上次輸入的設定
   - 下次啟動時自動載入

5. **使用者體驗**
   - 處理中鎖定 UI 防止誤操作
   - 顯示進度條和處理狀態
   - 支援取消操作

6. **自動複製靜態檔案**
   - 自動將差異檔案從 CI 建置目錄複製到上版目錄
   - 支援選擇建置結果資料夾與預計上版資料夾
   - 自動維持資料夾層級結構
   - 複製前自動檢查並提示目標資料夾狀態
   - 針對已存在的目標資料夾提供清空確認（含路徑提示）

7. **強制複製檔案清單**（新功能）
   - 支援手動指定要強制複製的檔案（如建置產生的 .dll、.exe 等二進位檔案）
   - 支援多行輸入，每行一個檔案路徑
   - **支援萬用字元匹配**：
     - `*` - 匹配任意數量的字元（例如：`bin\*.dll` 匹配所有 dll 檔案）
     - `?` - 匹配單一字元（例如：`app?.dll` 匹配 app1.dll、app2.dll）
     - `**` - 遞迴搜尋所有子目錄（例如：`lib\**\*.config` 遞迴搜尋所有 config 檔案）
   - 自動記憶上次輸入的檔案清單
   - 與 Git 差異檔案分開統計，清楚顯示複製來源

## 系統需求

- Windows 作業系統
- .NET 8.0 Runtime 或更高版本
- 網路連線（用於存取 Azure DevOps / TFS API）

## 專案結構

```
GitReleaseDiff/
├── GitReleaseDiff.sln               # 方案檔案
├── README.md                        # 本文件
├── doc/
│   └── Screenshot.png               # 執行畫面截圖
├── src/
│   └── GitReleaseDiff/              # 主專案
│       ├── Models/                  # 資料模型
│       │   ├── AppSettings.cs       # 應用程式設定
│       │   └── FileDiffInfo.cs      # 檔案差異資訊
│       ├── Services/                # 服務類別
│       │   ├── GitService.cs        # Git 操作服務
│       │   ├── SettingsService.cs   # 設定儲存服務
│       │   ├── CsvExportService.cs  # CSV 匯出服務
│       │   └── FileCopyService.cs   # 檔案複製服務
│       ├── MainForm.cs              # 主視窗邏輯
│       ├── MainForm.Designer.cs     # 主視窗設計
│       └── Program.cs               # 程式進入點
└── tests/
    └── GitReleaseDiff.Tests/        # 單元測試專案
        ├── FileDiffInfoTests.cs     # 檔案差異模型測試
        ├── GitServiceTests.cs       # Git 服務測試
        ├── CsvExportServiceTests.cs # CSV 匯出測試
        ├── SettingsServiceTests.cs  # 設定服務測試
        └── FileCopyServiceTests.cs  # 檔案複製測試
```

## 建置與執行

### 建置專案

```bash
cd GitReleaseDiff
dotnet build --configuration Release
```

### 執行測試

```bash
dotnet test --configuration Release
```

### 執行程式

```bash
dotnet run --project src/GitReleaseDiff
```

或直接執行編譯後的執行檔：
```
src\GitReleaseDiff\bin\Release\net8.0-windows\GitReleaseDiff.exe
```

## 使用說明

1. 啟動程式後，輸入以下資訊：
   - **Git 網址**：Azure DevOps / TFS 的 Git 儲存庫網址
     - 例如：`https://dev.azure.com/organization/ProjectName/_git/RepoName`
   - **Personal Access Token (PAT)**：Azure DevOps 的個人存取權杖（必填）
     - 取得方式：登入 Azure DevOps → 右上角使用者設定 → Personal access tokens → New Token
     - 所需權限：至少需要 `Code (Read)` 權限
     - 輸入後會以遮罩方式顯示（●●●）
   - **基準 Commit ID**：上次上版的 Commit ID（支援短碼）
   - **比對 Commit ID**：本次上版的 Commit ID（支援短碼）

2. 點擊「執行比對」按鈕

3. 等待處理完成，查看檔案差異列表

4. 如需匯出結果，點擊「匯出 CSV」按鈕

5. **自動複製檔案**（比對完成後）：
   - 選擇「建置結果資料夾」（CI Build Artifacts）
   - **（可選）** 輸入「專案路徑前綴」
     - 適用於多專案方案：若 Git 差異路徑包含專案資料夾前綴，但建置輸出不包含，請填寫此欄位
     - 例如：`Application` 或 `Application.UnitTest/`
   - 選擇「預計上版資料夾」（Deployment Folder）
   - **（可選）** 輸入「強制複製檔案清單」
     - 用於指定建置產生的二進位檔案（如 .dll、.exe）或其他不在 Git 差異中的檔案
     - 每行輸入一個檔案路徑，支援萬用字元：
       ```
       bin\MyApp.dll
       bin\*.exe
       lib\**\*.config
       plugins\**\*.dll
       ```
     - 萬用字元說明：
       - `bin\*.dll` - 匹配 bin 資料夾中所有 .dll 檔案
       - `lib\**\*.config` - 遞迴搜尋 lib 及其子資料夾中所有 .config 檔案
       - `app?.dll` - 匹配 app1.dll、app2.dll 等單字元變化
   - 點擊「執行複製」按鈕
   - 系統會：
     1. 複製 Git 差異中的檔案
     2. 複製強制指定的檔案
     3. 顯示詳細統計（分別列出兩種來源的複製結果）

6. 以下為應用程式執行畫面範例：
   ![執行畫面](doc/Screenshot.png) 
   ![輸出結果畫面](doc/Screenshot2.png)



### Personal Access Token (PAT) 安全提示

- PAT 會被加密儲存在本機，但仍建議定期更新
- 建議建立專用的 PAT，僅授予必要的權限（Code - Read）
- 不要與他人分享您的 PAT
- 如果 PAT 外洩，請立即在 Azure DevOps 中撤銷該 Token

## 使用的套件

| 套件名稱 | 版本 | 授權 | 用途 |
|---------|------|------|------|
| Newtonsoft.Json | 13.0.3 | MIT | JSON 序列化與反序列化 |
| System.Security.Cryptography.ProtectedData | 10.0.1 | MIT | Windows DPAPI 資料加密 |
| xUnit | 2.6.2 | Apache 2.0 | 單元測試框架 |

> 所有使用的套件皆為允許商用的開源授權。


## 修改歷程

| 版本 | 日期 | 說明 |
|------|------|------|
| 1.0.0 | 2025-12-31 | 初始版本發布 |
| 1.1.0 | 2026-01-01 | 新增自動複製上版差異檔案功能 |
| 1.2.0 | 2026-02-05 | 新增強制複製檔案清單功能，支援萬用字元（`*`、`?`、`**`）匹配二進位檔案及其他建置產物 | 

## 開發者

本專案遵循 Clean Code 原則開發，所有類別、屬性、方法皆使用繁體中文進行文件說明。

## 授權

本專案僅供內部使用。
