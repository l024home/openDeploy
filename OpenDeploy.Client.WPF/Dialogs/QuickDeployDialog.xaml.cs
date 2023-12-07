namespace OpenDeploy.Client.Dialogs
{
    public partial class QuickDeployDialog
    {
        public QuickDeployDialog(Models.SolutionViewModel solutionViewModel)
        {
            InitializeComponent();
            DataContext = solutionViewModel;
        }
    }
}
