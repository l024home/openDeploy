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
        Logger.Warn("Main");

        //启动画面
        var splashScreen = new SplashScreen("/Resources/Images/OpenDeploy.png");
        splashScreen.Show(true, true);

        //启动应用程序
        var app = new App()
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose,
            MainWindow = new MainWindow()
        };
        
        app.Run();
    }
}
