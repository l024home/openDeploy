﻿using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using OpenDeploy.Client.Dialogs;
using OpenDeploy.Client.Helper;
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
            Id = a.Id,
            GitRepositoryPath = a.GitRepositoryPath,
            SolutionName = a.SolutionName,
            Projects = a.Projects.Select(p => new ProjectViewModel
            {
                IsWeb = p.IsWeb,
                ProjectDir = p.ProjectDir,
                ProjectName = p.ProjectName,
                ReleaseDir = p.ReleaseDir,
            }).ToList()
        }).ToList();
        Solutions = solutionViewModels;
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

        //持久化到Sqlite
        await solutionRepository.AddSolutionAsync(solution);

        Growl.SuccessGlobal("操作成功");

        //重新加载解决方案
        await GetSolutionViewModelAsync();

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
                ProjectDir = projectDir!,
                ProjectName = projectName,
                IsWeb = webConfigFiles != null && webConfigFiles.Length > 0,
                SolutionName = solutionName,
                ReleaseDir = string.Empty
            };
            solution.Projects.Add(project);
        }
        return solution;
    }
}
