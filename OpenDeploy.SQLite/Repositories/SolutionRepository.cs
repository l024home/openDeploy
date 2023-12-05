using OpenDeploy.Domain.Models;

namespace OpenDeploy.SQLite;

/// <summary> 解决方案仓储 </summary>
public class SolutionRepository
{
    private readonly OpenDeployDbContext context;

    public SolutionRepository(OpenDeployDbContext context)
    {
        this.context = context;
        context.Database.EnsureCreated();
    }

    /// <summary> 添加解决方案 </summary>
    public void AddSolution(Solution solution)
    {
        context.Solutions.Add(solution);
        context.SaveChanges();
    }

    /// <summary> 获取所有的解决方案 </summary>
    public List<Solution> GetSolutions()
    {
        return [.. context.Solutions];
    }
}
