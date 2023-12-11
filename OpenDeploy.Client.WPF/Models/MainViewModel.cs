using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using OpenDeploy.Client.Dialogs;
using OpenDeploy.Domain.Models;
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
    private List<SolutionViewModel> solutions = default!;

    /// <summary> 解决方案Git路径 </summary>
    [ObservableProperty]
    private string solutionGitPath = string.Empty;

    /// <summary> 初始化 </summary>
    public async Task InitAsync()
    {
        await Task.Run(async () =>
        {
            await solutionRepository.InitAsync();
            await GetSolutionViewModelAsync();
        });
    }

    /// <summary> 加载解决方案视图模型 </summary>
    private async Task GetSolutionViewModelAsync()
    {
        var solutions = await solutionRepository.GetSolutionAsync();
        var solutionViewModels = solutions.Select(a => new SolutionViewModel
        {
            SolutionId = a.Id,
            GitRepositoryPath = a.GitRepositoryPath,
            SolutionName = a.SolutionName,
            Projects = a.Projects.Select(p => new ProjectViewModel
            {
                ProjectId = p.Id,
                ProjectName = p.ProjectName,
                ProjectDir = p.ProjectDir,
                ReleaseDir = p.ReleaseDir,
                IsWeb = p.IsWeb,
            }).ToList()
        }).ToList();

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Solutions = solutionViewModels;
        });
    }


    /// <summary> 配置解决方案弹窗 </summary>
    private Dialog? configSolutionDialog;

    /// <summary> 打开配置解决方案弹窗 </summary>
    [RelayCommand]
    public void OpenConfigSolutionDialog()
    {
        SolutionGitPath = string.Empty;
        configSolutionDialog = Dialog.Show(new SolutionConfigDialog(this));
    }

    /// <summary> 确定配置解决方案 </summary>
    [RelayCommand]
    private async Task OkConfigSolution()
    {
        try
        {
            if (!GitHelper.IsValidRepository(SolutionGitPath))
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

        //发现解决方案
        var solution = DetectSolution(SolutionGitPath);

        //持久化到数据库
        await solutionRepository.AddSolutionAsync(solution);

        //重新加载解决方案
        await GetSolutionViewModelAsync();

        Growl.SuccessGlobal("操作成功");

        //关闭弹窗
        configSolutionDialog?.Close();
    }


    /// <summary>
    /// 发现解决方案
    /// </summary>
    private static Solution DetectSolution(string gitRepoPath)
    {
        string[] solutionFilePaths = Directory.GetFiles(gitRepoPath, "*.sln", SearchOption.AllDirectories);
        if (solutionFilePaths == null || solutionFilePaths.Length == 0)
        {
            throw new Exception("未找到解决方案");
        }
        string[] projectFilePaths = Directory.GetFiles(gitRepoPath, "*.csproj", SearchOption.AllDirectories);
        if (projectFilePaths == null || projectFilePaths.Length == 0)
        {
            throw new Exception("未找到项目");
        }

        var solutionFilePath = solutionFilePaths[0];
        var solutionDir = Path.GetDirectoryName(solutionFilePath);
        var solutionName = Path.GetFileNameWithoutExtension(solutionFilePath);

        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            GitRepositoryPath = gitRepoPath,
            SolutionDir = solutionDir!,
            SolutionName = solutionName
        };

        foreach (var projectFilePath in projectFilePaths)
        {
            var projectDir = Path.GetDirectoryName(projectFilePath);
            var projectName = Path.GetFileNameWithoutExtension(projectFilePath);
            var webConfigFiles = Directory.GetFiles(projectDir!, "web.config", SearchOption.TopDirectoryOnly);
            var project = new Project
            {
                Id = Guid.NewGuid(),
                SolutionId = solution.Id,
                SolutionName = solution.SolutionName,
                ProjectName = projectName,
                ProjectDir = projectDir!,
                ReleaseDir = string.Empty,
                IsWeb = webConfigFiles != null && webConfigFiles.Length > 0,
            };
            solution.Projects.Add(project);
        }
        return solution;
    }
}
