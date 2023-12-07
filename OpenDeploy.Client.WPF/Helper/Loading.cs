using System.Windows;
using OpenDeploy.Client.Windows;

namespace OpenDeploy.Client.Helper;

public static class Loading
{
    /// <summary>
    /// 打开Loading窗体
    /// </summary>
    /// <param name="owner">Loading所有者</param>
    /// <param name="timeout">Loading超时时间/毫秒</param>
    /// <returns>Loading窗体</returns>
    public static LoadingWindow Show(Window? owner = null, int timeout = 5000, bool tranparent = true)
    {
        owner ??= Application.Current.MainWindow;
        var loadingWindow = new LoadingWindow(owner, timeout, tranparent);
        loadingWindow.Show();
        return loadingWindow;
    }

    /// <summary>
    /// 关闭Loading窗体
    /// </summary>
    /// <param name="loadingWindow">Loading窗体</param>
    public static void Hide(Window? loadingWindow = null)
    {
        if (loadingWindow == null)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoadingWindow loading)
                {
                    loading.Close();
                }
            }
        }
        else
        {
            loadingWindow.Close();
        }
    }

}
