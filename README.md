# 故事背景

小公司,单体项目,接口和页面都在一起,生产和测试环境都是 Windows 服务器和 IIS, 本地编译完成,把相关的页面和程序集拷贝到服务器上,尤其是涉及到多个页面,一个个页面找到对应的位置,再到服务器上找到对应的位置拷贝进去,甚至还有备份等操作,不胜其烦,因为历史遗留原因,项目是基于.net4.5 开发的,项目也比较大,基本不可能重写,也不可能每次都全量发布,文件很大,很慢,至于在服务器编译更不考虑,测试服务器内存只有 4G,跑了好几个项目,所有萌生了自己开发一个自动发布工具的想法。

# 总体设想

1. 通过 git 获取自上次发布以来修改的代码, 解析出需要发布的文件. (页面或 dll 等)
2. 封装待发布的文件为 DeployFileInfo, 记录文件类型,文件路径,和在服务器的文件路径等
3. 把所有文件打包为 zip, 通过 DotNetty 发送到服务器
4. 服务器解析 zip, 执行备份, 替换发布文件, 记录发布历史等

# 涉及的技术栈

-   **.NET 8.0**
-   **DotNetty**
-   **WPF**
-   **HandyControl**
-   **CommunityToolkit.Mvvm**
-   **Windows Service**
-   **IIS**
-   **Git**
-   **LibGit2Sharp**
-   **SQLite**

# 代码仓库

> 项目暂且就叫 `OpenDeploy` 吧

-   [OpenDeploy: https://gitee.com/broadm-dotnet/OpenDeploy](https://gitee.com/broadm-dotnet/OpenDeploy)

-   自动发布工具基于 VS2022 + .NET 8.0 + WPF + DotNetty
-   待测试的单体 Web 项目是 ASP.NET 4.8 的包含 MVC,WebApi,Webform 等
-   欢迎大家拍砖,Star

# 计划

> 一点点的实现构想中的内容,加油

# 完结效果图

![输入图片说明](OpenDeploy.Client.WPF/Resources/Images/%E5%8F%91%E5%B8%83%E9%A1%B9%E7%9B%AE%E9%85%8D%E7%BD%AE.png)
![输入图片说明](OpenDeploy.Client.WPF/Resources/Images/%E9%A6%96%E6%AC%A1%E5%8F%91%E5%B8%83.png)
![输入图片说明](OpenDeploy.Client.WPF/Resources/Images/%E6%9C%8D%E5%8A%A1%E5%99%A8%E6%97%A5%E5%BF%97.png)
