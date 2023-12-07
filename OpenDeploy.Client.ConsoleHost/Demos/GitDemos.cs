using LibGit2Sharp;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client.Demos;

/// <summary>
/// 测试git
/// </summary>
public class GitDemos
{


    /// <summary>
    /// 获取自上次提交以来的改动
    /// </summary>
    public static void GetChangesSinceLastPublish(string repoPath, string? id = null)
    {
        using var repo = new Repository(repoPath);

        //获取上次提交
        Commit? lastCommit;
        if (!string.IsNullOrEmpty(id))
        {
            lastCommit = repo.Lookup<Commit>(id);
        }
        else
        {
            lastCommit = repo.Commits.Last();
        }

        //获取自上次提交以来的改动
        var diff = repo.Diff.Compare<Patch>(lastCommit.Tree, DiffTargets.Index);

        Logger.Info("自上次提交以来的改动:");

        foreach (PatchEntryChanges item in diff)
        {
            Logger.Info($"{item.Status,10}    {item.Path}");
        }
    }
}
