using LibGit2Sharp;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client.Demos;

/// <summary>
/// 测试git
/// </summary>
public class GitDemos
{
    static string repoPath = "D:\\Projects\\Back\\dotnet\\Study\\OpenDeploy.TestWebProject";
    static string id = "fd67a7c800ce2f305fd11ace7aae85a5a4554b99";

    /// <summary>
    /// 获取自上次提交以来的改动
    /// </summary>
    public static void GetChangesSinceLastPublish()
    {
        using var repo = new Repository(repoPath);

        //获取上次发布的提交
        var parentCommit = repo.Lookup<Commit>(id);

        //获取自上次提交以来的改动
        var diff = repo.Diff.Compare<Patch>(parentCommit.Tree, DiffTargets.Index);

        Logger.Info("自上次提交以来的改动:");

        foreach (PatchEntryChanges item in diff)
        {
            Logger.Info($"{item.Status,10}    {item.Path}");
        }
    }
}
