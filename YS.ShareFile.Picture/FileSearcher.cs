using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using YS.Yuanji.Log;

public class IncrementalFileSearcher
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    /// <summary>
    /// 增量搜索文件
    /// </summary>
    /// <param name="rootPath">根路径</param>
    /// <param name="dateString">日期字符串</param>
    /// <param name="extensions">文件扩展名列表</param>
    /// <param name="keywords">关键词列表</param>
    /// <param name="stateFilePath">状态文件路径</param>
    /// <param name="includeNetworkShares">是否包含网络共享</param>
    /// <param name="timeoutSeconds">超时时间</param>
    /// <returns>搜索结果</returns>
    public static async Task<IncrementalSearchResult> SearchFilesIncrementalAsync(
        string rootPath,
        IEnumerable<string> dateString,
        IEnumerable<string> extensions,
        IEnumerable<string> keywords = null,
        Func<string,Task<bool>> exeUp = null,
        string stateFilePath = null,
        int maxDepth = 1,
        bool includeNetworkShares = true,
        int timeoutSeconds = 30)
    {
        var result = new IncrementalSearchResult();
        var searchStartTime = DateTime.Now;

        // 设置默认状态文件路径
        if (string.IsNullOrEmpty(stateFilePath))
        {
            stateFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FileSearcher",
                "search_state.json");
        }

        // 确保状态文件目录存在
        var stateDir = Path.GetDirectoryName(stateFilePath);
        if (!Directory.Exists(stateDir))
        {
            Directory.CreateDirectory(stateDir);
        }

        // 参数验证
        if (string.IsNullOrEmpty(rootPath))
        {
            result.ErrorMessage = "根路径不能为空";
            return result;
        }


        try
        {
            // 加载之前的搜索状态
            var previousState = await LoadSearchStateAsync(stateFilePath);
            var currentState = new SearchState
            {
                RootPath = rootPath,
                DateString = dateString?.ToList(),
                LastSearchTime = DateTime.Now
            };

            // 检查根目录是否存在
            if (!Directory.Exists(rootPath))
            {
                if (IsNetworkPath(rootPath) && includeNetworkShares)
                {
                    result.Warnings.Add($"根目录不存在，但可能是网络路径: {rootPath}");
                }
                else
                {
                    result.ErrorMessage = $"根路径不存在: {rootPath}";
                    return result;
                }
            }

            // 设置超时
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
            var searchTask = Task.Run(() =>
                PerformIncrementalSearch(rootPath, dateString, extensions, keywords,
                    includeNetworkShares, previousState, currentState, result, maxDepth));

            var completedTask = await Task.WhenAny(searchTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                result.IsTimeout = true;
                result.Warnings.Add($"搜索超时 ({timeoutSeconds}秒)");
            }
            else
            {
                await searchTask;
            }

            // 保存当前搜索状态
            if (!result.IsTimeout && string.IsNullOrEmpty(result.ErrorMessage))
            {
                currentState.ProcessedFiles = dateString == null ? previousState?.ProcessedFiles?.Where(f => f.LastWriteTime>=DateTime.Now.AddDays(-1))?.ToList()??new List<FileState>():previousState?.ProcessedFiles?.Where(f=>f.LastWriteTime >= dateString?.Select(d=>Convert.ToDateTime(d)).Min())?.ToList() ?? new List<FileState>();
                foreach (var file in result.NewFiles)
                {
                    try
                    {
                        if(await exeUp.Invoke(file))
                        {
                            var fileInfo = new FileInfo(file);
                            var currentFileState = new FileState
                            {
                                FilePath = file,
                                LastWriteTime = fileInfo.LastWriteTime,
                                FileSize = fileInfo.Length
                            };

                            currentState.ProcessedFiles.Add(currentFileState);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Warnings.Add($"执行文件{file}上传动作失败: {ex}");
                    }
                    
                }
                await SaveSearchStateAsync(currentState, stateFilePath);
                result.StateFilePath = stateFilePath;
            }

            result.PreviousSearchTime = previousState?.LastSearchTime;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"搜索过程中发生错误: {ex.Message}";
            result.Exception = ex;
        }
        finally
        {
            result.SearchDuration = DateTime.Now - searchStartTime;
        }

        return result;
    }

    private static void PerformIncrementalSearch(
        string rootPath,
        IEnumerable<string> dateString,
        IEnumerable<string> extensions,
        IEnumerable<string> keywords,
        bool includeNetworkShares,
        SearchState previousState,
        SearchState currentState,
        IncrementalSearchResult result,
        int maxDepth = 1)
    {
        // 第一步：找到所有包含指定日期的目录
        var dateDirectories = FindDirectoriesWithDate(rootPath, dateString, includeNetworkShares, result,0, maxDepth);
        result.DateDirectoriesCount = dateDirectories.Count;
        //result.InfoMessages.Add($"找到 {dateDirectories.Count} 个包含日期 '{string.Join(",",dateString)}' 的目录");

        // 第二步：在每个包含日期的目录及其子目录中增量搜索文件
        foreach (string dateDir in dateDirectories)
        {
            if (result.IsTimeout) break;

            SearchFilesIncrementalInDirectory(
                dateDir, extensions, keywords, includeNetworkShares, previousState, currentState, result);
        }

        // 统计增量结果
        result.NewFilesCount = result.NewFiles.Count;
        result.ModifiedFilesCount = result.ModifiedFiles.Count;
        result.UnchangedFilesCount = result.UnchangedFiles.Count;
    }

    /// <summary>
    /// 递归查找所有包含指定日期的目录
    /// </summary>
    private static List<string> FindDirectoriesWithDate(
        string currentPath,
        IEnumerable<string> dateString,
        bool includeNetworkShares,
        IncrementalSearchResult result,
        int currentDepth, int maxDepth
         )
    {
        var dateDirectories = new List<string>();

        try
        {
            if (currentDepth > maxDepth)
            {
                return dateDirectories;
            }

            // 检查当前目录是否包含指定日期
            if (DirectoryContainsDate(currentPath, dateString))
            {
                dateDirectories.Add(currentPath);
                return dateDirectories;
            }

            // 检查是否是网络路径且是否应该跳过
            if (IsNetworkPath(currentPath) && !includeNetworkShares)
            {
                result.Warnings.Add($"跳过网络路径（根据配置）: {currentPath}");
            }

            // 递归查找子目录
            foreach (string subDirectory in SafeGetDirectories(currentPath, result))
            {
                dateDirectories.AddRange(
                    FindDirectoriesWithDate(subDirectory, dateString, includeNetworkShares, result,currentDepth+1,maxDepth));
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            result.Warnings.Add($"无权限访问目录: {currentPath} - {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            result.Warnings.Add($"目录不存在: {currentPath} - {ex.Message}");
        }
        catch (IOException ex) when (IsNetworkRelatedException(ex))
        {
            result.Warnings.Add($"网络错误访问目录: {currentPath} - {ex.Message}");
        }
        catch (Exception ex)
        {
            result.Warnings.Add($"访问目录时发生错误: {currentPath} - {ex.Message}");
        }

        return dateDirectories;
    }

    /// <summary>
    /// 在指定目录及其所有子目录中增量搜索文件
    /// </summary>
    private static void SearchFilesIncrementalInDirectory(
        string directoryPath,
        IEnumerable<string> extensions,
        IEnumerable<string> keywords,
        bool includeNetworkShares,
        SearchState previousState,
        SearchState currentState,
        IncrementalSearchResult result)
    {
        try
        {
            // 检查是否是网络路径且是否应该跳过
            if (IsNetworkPath(directoryPath) && !includeNetworkShares)
            {
                return;
            }

            // 搜索当前目录中的文件
            SearchFilesIncrementalInSingleDirectory(
                directoryPath, extensions, keywords, previousState, currentState, result);

            // 递归搜索子目录
            foreach (string subDirectory in SafeGetDirectories(directoryPath, result))
            {
                if (result.IsTimeout) break;

                SearchFilesIncrementalInDirectory(
                    subDirectory, extensions, keywords, includeNetworkShares, previousState, currentState, result);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            result.Warnings.Add($"无权限访问目录: {directoryPath} - {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            result.Warnings.Add($"目录不存在: {directoryPath} - {ex.Message}");
        }
        catch (IOException ex) when (IsNetworkRelatedException(ex))
        {
            result.Warnings.Add($"网络错误访问目录: {directoryPath} - {ex.Message}");
        }
        catch (Exception ex)
        {
            result.Warnings.Add($"访问目录时发生错误: {directoryPath} - {ex.Message}");
        }
    }

    /// <summary>
    /// 在单个目录中增量搜索文件
    /// </summary>
    private static void SearchFilesIncrementalInSingleDirectory(
        string directoryPath,
        IEnumerable<string> extensions,
        IEnumerable<string> keywords,
        SearchState previousState,
        SearchState currentState,
        IncrementalSearchResult result)
    {
        try
        {
            var files = SafeGetFiles(directoryPath, result);

            foreach (string file in files)
            {
                try
                {
                    if (!IsFileMatch(file, extensions, keywords))
                        continue;

                    // 记录所有处理的文件
                    result.AllProcessedFiles.Add(file);

                    // 获取文件信息用于增量判断
                    var fileInfo = new FileInfo(file);
                    if (!fileInfo.Exists)
                        continue;

                    //currentState.ProcessedFiles.Add(currentFileState);
                    // 查找之前的文件状态
                    var previousFileState = previousState?.ProcessedFiles
                        ?.FirstOrDefault(f => f.FilePath.Equals(file, StringComparison.OrdinalIgnoreCase));

                    if (previousFileState == null)
                    {
                        // 新文件
                        result.NewFiles.Add(file);
                        result.FoundFiles.Add(file);
                    }
                    else if (previousFileState.LastWriteTime != fileInfo.LastWriteTime ||
                             previousFileState.FileSize != fileInfo.Length)
                    {
                        // 修改过的文件
                        result.ModifiedFiles.Add(file);
                        result.FoundFiles.Add(file);
                    }
                    else
                    {
                        // 未变化的文件
                        result.UnchangedFiles.Add(file);
                    }
                }
                catch (PathTooLongException)
                {
                    result.Warnings.Add($"路径太长，跳过文件: {file}");
                }
                catch (Exception ex)
                {
                    result.Warnings.Add($"处理文件时发生错误: {file} - {ex.Message}");
                }
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            result.Warnings.Add($"无权限访问目录中的文件: {directoryPath} - {ex.Message}");
        }
        catch (IOException ex) when (IsNetworkRelatedException(ex))
        {
            result.Warnings.Add($"网络错误访问目录: {directoryPath} - {ex.Message}");
        }
        catch (Exception ex)
        {
            result.Warnings.Add($"访问目录时发生错误: {directoryPath} - {ex.Message}");
        }
    }

    /// <summary>
    /// 安全获取目录列表，处理异常
    /// </summary>
    private static string[] SafeGetDirectories(string path, IncrementalSearchResult result)
    {
        try
        {
            return Directory.GetDirectories(path);
        }
        catch (Exception ex)
        {
            result.Warnings.Add($"获取子目录列表失败: {path} - {ex.Message}");
            return new string[0];
        }
    }

    /// <summary>
    /// 安全获取文件列表，处理异常
    /// </summary>
    private static string[] SafeGetFiles(string path, IncrementalSearchResult result)
    {
        try
        {
            return Directory.GetFiles(path);
        }
        catch (Exception ex)
        {
            result.Warnings.Add($"获取文件列表失败: {path} - {ex.Message}");
            return new string[0];
        }
    }

    /// <summary>
    /// 加载搜索状态
    /// </summary>
    private static async Task<SearchState> LoadSearchStateAsync(string stateFilePath)
    {
        try
        {
            if (!File.Exists(stateFilePath))
                return null;

            var json = await File.ReadAllTextAsync(stateFilePath);
            return JsonSerializer.Deserialize<SearchState>(json, JsonOptions);
        }
        catch (Exception)
        {
            // 如果状态文件损坏，返回空状态
            return null;
        }
    }

    /// <summary>
    /// 保存搜索状态
    /// </summary>
    private static async Task SaveSearchStateAsync(SearchState state, string stateFilePath)
    {
        try
        {
            var json = JsonSerializer.Serialize(state, JsonOptions);
            await File.WriteAllTextAsync(stateFilePath, json);
        }
        catch (Exception ex)
        {
            throw new Exception($"保存搜索状态失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 清除搜索状态
    /// </summary>
    public static async Task ClearSearchStateAsync(string stateFilePath = null)
    {
        if (string.IsNullOrEmpty(stateFilePath))
        {
            stateFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FileSearcher",
                "search_state.json");
        }

        if (File.Exists(stateFilePath))
        {
            await Task.Run(() => File.Delete(stateFilePath));
        }
    }

    private static bool DirectoryContainsDate(string directoryPath, IEnumerable<string> dateString)
    {
        string directoryName = Path.GetFileName(directoryPath);
        return dateString == null || dateString.Any(d=>directoryName.IndexOf(d, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static bool IsFileMatch(string filePath, IEnumerable<string> extensions, IEnumerable<string> keywords)
    {
        string fileName = Path.GetFileName(filePath);
        string extension = Path.GetExtension(filePath);

        // 检查文件扩展名
        bool extensionMatch = extensions == null || extensions.Any(ext =>
            string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase));

        if (!extensionMatch) return false;

        // 检查关键词（如果提供了关键词）
        if (keywords != null && keywords.Any())
        {
            bool keywordMatch = keywords.Any(keyword =>
                fileName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

            if (!keywordMatch) return false;
        }

        return true;
    }

    private static bool IsNetworkPath(string path)
    {
        try
        {
            if (path.StartsWith(@"\\") || path.StartsWith("//"))
                return true;

            var root = Path.GetPathRoot(path);
            if (!string.IsNullOrEmpty(root))
            {
                var drive = new DriveInfo(root);
                return drive.DriveType == DriveType.Network;
            }
        }
        catch
        {
            // 如果无法确定，假设不是网络路径
        }
        return false;
    }

    private static bool IsNetworkRelatedException(Exception ex)
    {
        return ex is IOException &&
               (ex.Message.Contains("网络") ||
                ex.Message.Contains("network") ||
                ex.Message.Contains("远程") ||
                ex.Message.Contains("remote"));
    }
}

/// <summary>
/// 搜索状态
/// </summary>
public class SearchState
{
    public string? RootPath { get; set; }
    public List<string>? DateString { get; set; }
    public DateTime LastSearchTime { get; set; }
    public List<FileState> ProcessedFiles { get; set; } = new List<FileState>();
}

/// <summary>
/// 文件状态
/// </summary>
public class FileState
{
    public string FilePath { get; set; }
    public DateTime LastWriteTime { get; set; }
    public long FileSize { get; set; }
}

/// <summary>
/// 增量搜索结果
/// </summary>
public class IncrementalSearchResult
{
    // 所有找到的文件（新文件+修改的文件）
    public List<string> FoundFiles { get; set; } = new List<string>();

    // 分类的文件列表
    public List<string> NewFiles { get; set; } = new List<string>();
    public List<string> ModifiedFiles { get; set; } = new List<string>();
    public List<string> UnchangedFiles { get; set; } = new List<string>();

    // 所有处理的文件（用于下次增量搜索）
    public HashSet<string> AllProcessedFiles { get; set; } = new HashSet<string>();

    // 统计信息
    public int NewFilesCount { get; set; }
    public int ModifiedFilesCount { get; set; }
    public int UnchangedFilesCount { get; set; }
    public int DateDirectoriesCount { get; set; }

    // 状态信息
    public List<string> Warnings { get; set; } = new List<string>();
    public List<string> InfoMessages { get; set; } = new List<string>();
    public string ErrorMessage { get; set; }
    public Exception Exception { get; set; }
    public bool IsTimeout { get; set; }
    public TimeSpan SearchDuration { get; set; }
    public DateTime? PreviousSearchTime { get; set; }
    public string StateFilePath { get; set; }

    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage) && !IsTimeout;
    public int TotalFoundFiles => FoundFiles.Count;

    public void PrintSummary()
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            
            LogController.Instance.Log($"错误: {ErrorMessage}");
            if (Exception != null)
            {
                LogController.Instance.Log($"异常详情: {Exception}");
            }
        }

        if (IsTimeout)
        {
            LogController.Instance.Log("搜索超时");
        }

        LogController.Instance.Log($"搜索耗时: {SearchDuration.TotalSeconds:F2}秒");
        LogController.Instance.Log($"找到包含日期的目录: {DateDirectoriesCount}个");
        LogController.Instance.Log($"处理文件总数: {AllProcessedFiles.Count}个");
        LogController.Instance.Log($"发现新文件: {NewFilesCount}个");
        LogController.Instance.Log($"发现修改文件: {ModifiedFilesCount}个");
        LogController.Instance.Log($"未变化文件: {UnchangedFilesCount}个");
        LogController.Instance.Log($"总匹配文件: {TotalFoundFiles}个");

        if (PreviousSearchTime.HasValue)
        {
            LogController.Instance.Log($"上次搜索时间: {PreviousSearchTime.Value:yyyy-MM-dd HH:mm:ss}");
        }

        if (!string.IsNullOrEmpty(StateFilePath))
        {
            LogController.Instance.Log($"状态文件: {StateFilePath}");
        }

        if (Warnings.Any())
        {
            LogController.Instance.Log($"\n警告数量: {Warnings.Count}个");
            foreach (var warning in Warnings.Take(5))
            {
                LogController.Instance.Log($"  - {warning}");
            }
            if (Warnings.Count > 5)
            {
                LogController.Instance.Log($"  ... 还有 {Warnings.Count - 5} 个警告未显示");
            }
        }

        if (InfoMessages.Any())
        {
            LogController.Instance.Log($"\n消息数量: {InfoMessages.Count}个");
            foreach (var info in InfoMessages)
            {
                LogController.Instance.Log($"  - {info}");
            }
            //if (Warnings.Count > 5)
            //{
            //    LogController.Instance.Log($"  ... 还有 {Warnings.Count - 5} 个警告未显示");
            //}
        }

        if (NewFiles.Any())
        {
            LogController.Instance.Log("\n新文件:");
            foreach (var file in NewFiles.Take(10))
            {
                LogController.Instance.Log($"  + {file}");
            }
            if (NewFiles.Count > 10)
            {
                LogController.Instance.Log($"  ... 还有 {NewFiles.Count - 10} 个新文件未显示");
            }
        }

        if (ModifiedFiles.Any())
        {
            LogController.Instance.Log("\n修改的文件:");
            foreach (var file in ModifiedFiles.Take(10))
            {
                LogController.Instance.Log($"  * {file}");
            }
            if (ModifiedFiles.Count > 10)
            {
                LogController.Instance.Log($"  ... 还有 {ModifiedFiles.Count - 10} 个修改文件未显示");
            }
        }
    }
}
