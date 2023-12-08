using OpenDeploy.Communication.Convention;
using OpenDeploy.Domain.Convention;

namespace OpenDeploy.Domain.NettyHeaders;

/// <summary>
/// 发布请求头部
/// </summary>
public class DeployRequestHeader : NettyHeader
{
    public DeployRequestHeader() : base("Deploy/Run") { }
    public List<DeployFileInfo> Files { get; set; } = [];
    public string ProjectName { get; set; } = string.Empty;
    public string SolutionName { get; set; } = string.Empty;
    public string ZipFileName { get; set; } = string.Empty;
}
