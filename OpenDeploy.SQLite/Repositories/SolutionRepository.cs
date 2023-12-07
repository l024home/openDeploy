using Microsoft.EntityFrameworkCore;
using OpenDeploy.Domain.Models;

namespace OpenDeploy.SQLite;

/// <summary> 解决方案仓储 </summary>
public class SolutionRepository
{
    private readonly OpenDeployDbContext context;

    public SolutionRepository(OpenDeployDbContext context)
    {
        this.context = context;
    }

    /// <summary> 添加解决方案 </summary>
    public void AddSolution(Solution solution)
    {
        context.Solutions.Add(solution);
        context.SaveChanges();
    }

    /// <summary> 获取所有的解决方案 </summary>
    public async Task<List<Solution>> GetSolutionAsync()
    {
        await context.Database.EnsureCreatedAsync();
        return await context.Solutions.Include(a => a.Projects).ToListAsync();
    }

    /// <summary> 获取上次的提交记录 </summary>
    public PublishRecord? GetLastCommit(int solutionId)
    {
        return context.PublishRecords.Where(a => a.SolutionId == solutionId).OrderByDescending(a => a.PublishTime).FirstOrDefault();
    }
}
