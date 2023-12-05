using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// 获取git修改记录
    /// </summary>
    /// <param name="repoPath">git仓库路径</param>
    /// <param name="since">起始时间</param>
    /// <returns>修改记录</returns>
    public static Task<List<CommitInfoDto>> GetLogsAsync(string repoPath, DateTime since, string authorName)
    {
        return Task.Run(() =>
        {
            var commitDtos = new List<CommitInfoDto>();
            var repo = GetRepo(repoPath);


            var commits = repo.Commits
                              .Where(c =>

                              c.Author.Name == authorName
                              && c.Author.When.DateTime > since
                              && !c.Message.StartsWith("Merge")
                              )
                              .ToList();

            foreach (Commit c in commits)
            {
                //Console.WriteLine("{0} | {1}", c.Message, c.MessageShort);
                //Console.WriteLine(string.Format("提交人: {0} 提交时间: {1}", c.Author.Name, c.Author.When.DateTime.ToString("yyyy-MM-dd HH:mm")));

                var changedFilePaths = new List<CommitInfoDto>();

                foreach (var parent in c.Parents)
                {
                    var changes = repo.Diff.Compare<TreeChanges>(parent.Tree, c.Tree);
                    foreach (TreeEntryChanges change in changes)
                    {
                        changedFilePaths.Add(new CommitInfoDto { Name = change.Path });
                    }
                }

                commitDtos.Add(new CommitInfoDto
                {
                    Name = c.MessageShort,
                    DataList = changedFilePaths
                });
            }

            return commitDtos;
        });
    }


    public static void GetChangedFromCommit(string repoPath, string hash)
    {
        var repo = GetRepo(repoPath);


        var c = repo.Lookup<Commit>(hash);

        // Let's only consider the refs that lead to this commit...
        var refs = repo.Refs.ReachableFrom(new[] { c });

        //...and create a filter that will retrieve all the commits...
        var cf = new CommitFilter
        {

            IncludeReachableFrom = refs, // ...reachable from all those refs...

        };

        var cs = repo.Commits.QueryBy(cf);

        foreach (var co in cs)
        {
            Console.WriteLine("{0}: {1}", co.Id.ToString(7), co.MessageShort);
        }

    }


    public class CommitInfoDto
    {
        public string Name { get; set; } = default!;
        public List<CommitInfoDto> DataList { get; set; } = default!;
    }
}
