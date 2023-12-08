﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenDeploy.Communication.Convention;
using OpenDeploy.Domain.Convention;
using OpenDeploy.Domain.NettyHeaders;
using OpenDeploy.Infrastructure;
using OpenDeploy.Infrastructure.Extensions;

namespace OpenDeploy.Server.Handlers;

/// <summary>
/// 服务器发布文件处理器
/// </summary>
public class DeployHandler(NettyContext context) : AbstractNettyHandler(context)
{
    private static string EnsureBackupDirCreated(string zipFileName)
    {
        var backupDir = Path.Combine(Environment.CurrentDirectory, "Backup", zipFileName);
        if (!Directory.Exists(backupDir))
        {
            Directory.CreateDirectory(backupDir);
        }
        return backupDir;
    }

    /// <summary>
    /// 执行服务端发布
    /// </summary>
    /// <param name="model"></param>
    public void Run(DeployRequestHeader model)
    {
        Logger.Warn($"收到客户端的消息: {model.ToJsonString(true)}");

        var configs = NettyServer.AppHost.Services.GetRequiredService<IOptions<List<ProjectConfig>>>();
        var projectConfig = configs.Value.FirstOrDefault(a => a.ProjectName == model.ProjectName);
        if (projectConfig == null)
        {
            Logger.Error("请现在服务器项目的appsettings.json中配置项目信息");
            return;
        }

        var zipBytes = Request.Body;
        if (zipBytes == null || zipBytes.Length == 0)
        {
            Logger.Error("ZipBytes为空");
            return;
        }

        var zipFileName = model.ZipFileName;
        if (string.IsNullOrEmpty(zipFileName))
        {
            Logger.Error("ZipFileName为空");
            return;
        }

        //解压
        var zipDir = ZipHelper.UnZip(zipBytes, zipFileName);

        Logger.Info($"解压成功: {zipDir}");

        //备份并覆盖旧文件
        DoPublish(model.Files, zipDir, zipFileName, projectConfig);

        Logger.Info($"发布成功: {zipDir}");
    }


    /// <summary>
    /// 备份并覆盖旧文件
    /// </summary>
    private void DoPublish(List<DeployFileInfo> files, string zipFolder, string zipFileName, ProjectConfig projectConfig)
    {
        try
        {
            //先创建备份文件夹
            var backupDir = EnsureBackupDirCreated(zipFileName);

            foreach (DeployFileInfo file in files)
            {
                //源文件路径(解压后的文件)
                var sourceFileName = Path.Combine(zipFolder, file.PublishFileRelativePath);

                //待发布的文件路径 (服务器真实文件路径)
                var destFileName = Path.Combine(projectConfig.ProjectDir, file.PublishFileRelativePath);

                //服务器已存在此文件,先执行备份
                if (File.Exists(destFileName))
                {
                    //备份文件路径
                    var backupFileName = Path.Combine(backupDir, file.PublishFileRelativePath);
                    var backupFileDir = Path.GetDirectoryName(backupFileName);
                    if (!Directory.Exists(backupFileDir))
                    {
                        Directory.CreateDirectory(backupFileDir!);
                    }
                    File.Copy(destFileName, backupFileName);
                }
                else
                {
                    //服务器不存在此文件,先创建文件夹层级
                    var destFileDir = Path.GetDirectoryName(destFileName);
                    if (!Directory.Exists(destFileDir))
                    {
                        Directory.CreateDirectory(destFileDir!);
                    }
                }

                //替换服务器文件
                File.Copy(sourceFileName, destFileName, true);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
    }
}
