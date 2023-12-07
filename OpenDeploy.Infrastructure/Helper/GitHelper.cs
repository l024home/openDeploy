using LibGit2Sharp;

namespace OpenDeploy.Infrastructure;

/// <summary>
/// Git 操作帮助类
/// </summary>
public class GitHelper
{
    /// <summary>
    /// Git仓储缓存
    /// </summary>
    private static readonly Dictionary<string, Repository> cache = [];

    /// <summary>
    /// 是否有效的Git仓库
    /// </summary>
    /// <param name="repoPath">仓库路径</param>
    public static bool IsValidRepository(string repoPath) => Repository.IsValid(repoPath);

    /// <summary>
    /// 根据路径获取Git仓库
    /// </summary>
    /// <param name="repoPath">仓库路径</param>
    public static Repository GetRepo(string repoPath)
    {
        if (string.IsNullOrEmpty(repoPath))
        {
            throw new ArgumentNullException(nameof(repoPath));
        }
        if (cache.TryGetValue(repoPath, out Repository? value))
        {
            return value;
        }
        if (!IsValidRepository(repoPath))
        {
            throw new Exception("Invalid Git Repository");
        }
        var repo = new Repository(repoPath);
        cache.Add(repoPath, repo);
        return repo;
    }

    /// <summary>
    /// 获取自上次提交以来的改动
    /// </summary>
    public static List<PatchEntryChanges> GetChangesSinceLastPublish(string repoPath, string? lastCommitId = null)
    {
        var repo = GetRepo(repoPath);

        //获取上次发布的提交
        Commit? lastCommit = null;
        if (!string.IsNullOrEmpty(lastCommitId))
        {
            lastCommit = repo.Lookup<Commit>(lastCommitId);
            if (lastCommit == null)
            {
                throw new Exception("无法获取上次发布的提交记录");
            }
        }

        //获取自上次提交以来的改动
        var diff = repo.Diff.Compare<Patch>(lastCommit?.Tree, DiffTargets.Index);
        return [.. diff];
    }
}
