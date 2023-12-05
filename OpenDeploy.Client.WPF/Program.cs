using System.Windows;
using OpenDeploy.Client.WPF;
using OpenDeploy.Infrastructure;

namespace OpenDeploy.Client;

class Program
{
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    public static void Main()
    {
        //启动画面
        new SplashScreen("/Resources/Images/OpenDeploy.png").Show(true, true);

        //启动应用程序
        new App()
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose,
            MainWindow = new MainWindow()
        }.Run();
    }
}
