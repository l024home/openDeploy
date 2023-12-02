using System.Windows;
using OpenDeploy.Client.Windows;

namespace OpenDeploy.Client.Helper;

public static class Loading
{
    public static LoadingWindow Show(Window? owner = null)
    {
        owner ??= Application.Current.MainWindow;
        var loadingWindow = new LoadingWindow(owner);
        loadingWindow.Show();
        return loadingWindow;
    }

    public static void Hide(LoadingWindow? loadingWindow = null)
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
