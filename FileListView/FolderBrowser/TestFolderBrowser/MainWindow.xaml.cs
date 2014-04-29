namespace TestFolderBrowser
{
  using System.Windows;
  using FolderBrowser.ViewModel;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();
    }

    /// <summary>
    /// Use a button click event to demo the folder browser dialog...
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
      FolderBrowser.FolderBrowserDialog dlg = new FolderBrowser.FolderBrowserDialog();

      var dlgViewModel = new DialogViewModel();
      dlgViewModel.TreeBrowser.SelectedFolder = @"C:\";

      dlg.DataContext = dlgViewModel;

      bool? bResult = dlg.ShowDialog();

      if (dlgViewModel.DialogCloseResult == true || bResult == true)
        System.Windows.MessageBox.Show("OPening path:" + dlgViewModel.TreeBrowser.SelectedFolder);
    }
  }
}
