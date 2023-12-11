using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenDeploy.Domain.Models;

/// <summary> 发布记录领域模型 </summary>
[Table("PublishRecord")]
public class PublishRecord
{
    [Key]
    public Guid Id { get; set; }

    /// <summary> 解决方案ID </summary>
    public Guid SolutionId { get; set; }

    /// <summary> 解决方案名称 </summary>
    public string SolutionName { get; set; }  = string.Empty;

    /// <summary> 本次发布对应的Git提交ID </summary>
    public string GitCommitId { get; set; } = string.Empty;

    /// <summary> 发布时间 </summary>
    public DateTime PublishTime { get; set; }
}
