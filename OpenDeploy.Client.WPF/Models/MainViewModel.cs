using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpenDeploy.Client.Models;

public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// MVVM模式的可观测属性
    /// </summary>
    [ObservableProperty]
    private string counter = "随机数";

    /// <summary>
    /// 供视图界面调用的Command定义
    /// </summary>
    [RelayCommand]
    private void IncrementCounter()
    {
        Counter = Random.Shared.NextDouble().ToString();
    }
}
