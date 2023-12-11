using Microsoft.EntityFrameworkCore;
using OpenDeploy.Domain.Models;

namespace OpenDeploy.SQLite;

/// <summary> 解决方案仓储 </summary>
public class SolutionRepository(OpenDeployDbContext context)
{
    private readonly OpenDeployDbContext context = context;

    /// <summary> 初始化数据库 </summary>
    public async Task InitAsync()
    {
        await context.Database.EnsureCreatedAsync();
    }

    /// <summary> 添加解决方案 </summary>
    public async Task AddSolutionAsync(Solution solution)
    {
        await context.Solutions.AddAsync(solution);
        await context.SaveChangesAsync();
    }

    /// <summary> 获取所有的解决方案 </summary>
    public async Task<List<Solution>> GetSolutionAsync()
    {
        return await context.Solutions.Include(a => a.Projects).ToListAsync();
    }

    /// <summary> 更新项目的发布目录 </summary>
    public async Task UpdateProjectReleaseDir(Guid projectId, string dir)
    {
        var project = await context.Projects.FirstOrDefaultAsync(a => a.Id == projectId);
        if (project == null)
        {
            throw new Exception("为找到相关的项目信息");
        }
        project.ReleaseDir = dir;
        await context.SaveChangesAsync();
    }

    /// <summary> 获取上次发布记录 </summary>
    public async Task<PublishRecord?> GetLastPublishAsync(Guid solutionId)
    {
        return await context.PublishRecords
                            .Where(a => a.SolutionId == solutionId)
                            .OrderByDescending(a => a.PublishTime)
                            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 手动添加第一次发布记录
    /// </summary>
    public async Task SaveFirstPublishAsync(Guid solutionId, string solutionName, string commitId)
    {
        var record = new PublishRecord
        {
            Id = Guid.NewGuid(),
            SolutionId = solutionId,
            SolutionName = solutionName,
            GitCommitId = commitId,
            PublishTime = DateTime.Now,
        };
        await context.PublishRecords.AddAsync(record);
        await context.SaveChangesAsync();
    }
}
