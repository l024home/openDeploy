using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using OpenDeploy.Client.Dialogs;
using OpenDeploy.Domain.Models;
using OpenDeploy.Infrastructure;
using OpenDeploy.SQLite;

namespace OpenDeploy.Client.Models;

/// <summary> 主窗体视图模型 </summary>
public partial class MainViewModel : ObservableObject
{
    /// <summary> 解决方案仓储 </summary>
    public SolutionRepository SolutionRepository { get; }

    /// <summary> 解决方案集合 </summary>
    [ObservableProperty]
    private ObservableCollection<SolutionViewModel> solutions;

    /// <summary> 临时解决方案(用于配置) </summary>
    [ObservableProperty]
    private SolutionViewModel configSolution;

    public MainViewModel(SolutionRepository solutionRepository)
    {
        SolutionRepository = solutionRepository;
        configSolution = new SolutionViewModel();

        //加载已配置的解决方案
        var solutionEntities = solutionRepository.GetSolutions();
        var solutionViewModels = solutionEntities.Select(a => new SolutionViewModel
        {
            Id = a.Id,
            GitRepositoryPath = a.GitRepositoryPath,
            SolutionName = a.SolutionName,
        });
        solutions = new ObservableCollection<SolutionViewModel>(solutionViewModels);
    }

    /// <summary>
    /// 打开配置解决方案弹窗
    /// </summary>
    [RelayCommand]
    public void OpenConfigSolutionDialog()
    {
        Logger.Warn("OpenConfigSolutionDialog");
        Dialog.Show(new SolutionConfigDialog(this));
    }


    /// <summary>
    /// 确定配置解决方案
    /// </summary>
    [RelayCommand]
    private void OkConfigSolution()
    {
        try
        {
            if (string.IsNullOrEmpty(ConfigSolution.SolutionName))
            {
                throw new Exception("请填写解决方案名称");
            }
            if (!GitHelper.IsValidRepository(ConfigSolution.GitRepositoryPath))
            {
                throw new Exception("非法的Git仓储路径");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message);
            Growl.WarningGlobal(ex.Message);
            return;
        }

        Growl.SuccessGlobal("操作成功");


        ConfigSolution.Id = 0;
        ConfigSolution.SolutionName = "";
        ConfigSolution.GitRepositoryPath = "";
    }
}
