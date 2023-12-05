using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenDeploy.Client.Models;

/// <summary> 解决方案视图模型 </summary>
public partial class SolutionViewModel : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string solutionName = string.Empty;

    [ObservableProperty]
    public string gitRepositoryPath = string.Empty;
}
