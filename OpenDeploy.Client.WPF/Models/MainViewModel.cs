using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using OpenDeploy.Client.Dialogs;
using OpenDeploy.Infrastructure;
using OpenDeploy.SQLite;

namespace OpenDeploy.Client.Models;

/// <summary> 主窗体视图模型 </summary>
public partial class MainViewModel(SolutionRepository solutionRepository) : ObservableObject
{
    /// <summary> 解决方案仓储 </summary>
    private readonly SolutionRepository solutionRepository = solutionRepository;

    /// <summary> 解决方案集合 </summary>
    [ObservableProperty]
    private ObservableCollection<SolutionViewModel> solutions = default!;

    /// <summary> 临时解决方案(用于新增配置) </summary>
    [ObservableProperty]
    private SolutionViewModel configSolution = new ();

    /// <summary> 初始化解决方案 </summary>
    public void InitSolutions()
    {
        LoadSolutions();
    }

    /// <summary> 加载解决方案 </summary>
    private void LoadSolutions()
    {
        var solutionEntities = solutionRepository.GetSolutions();
        var solutionViewModels = solutionEntities.Select(a => new SolutionViewModel
        {
            Id = a.Id,
            GitRepositoryPath = a.GitRepositoryPath,
            SolutionName = a.SolutionName,
        });
        Solutions = new ObservableCollection<SolutionViewModel>(solutionViewModels);
    }


    /// <summary> 配置解决方案弹窗 </summary>
    private Dialog? configSolutionDialog;

    /// <summary> 打开配置解决方案弹窗 </summary>
    [RelayCommand]
    public void OpenConfigSolutionDialog()
    {
        ConfigSolution.Clear();
        configSolutionDialog = Dialog.Show(new SolutionConfigDialog(this));
    }

    /// <summary> 确定配置解决方案 </summary>
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
            Growl.ClearGlobal();
            Growl.WarningGlobal(ex.Message);
            return;
        }

        //持久化到Sqlite
        solutionRepository.AddSolution(ConfigSolution.Map2Entity());

        Growl.SuccessGlobal("操作成功");

        //重新加载解决方案
        LoadSolutions();

        //关闭弹窗
        configSolutionDialog?.Close();
    }
}
