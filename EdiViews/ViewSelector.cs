namespace EdiViews
{
  using System;
  using System.Windows;

  /// <summary>
  /// This class matches a viewmodel type with its corresponding dialog view.
  /// </summary>
  public class ViewSelector
  {
    /// <summary>
    /// Return a new view object for a matching type of ViewModel object
    /// (this limits the dependencies between view and viewmodel down to one)
    /// </summary>
    public static Window GetDialogView(object viewModel, Window parent = null)
    {
      if (viewModel == null)
        throw new Exception("The viewModel parameter cannot be null.");

      Window win = null;

      if (viewModel is EdiViews.Config.ViewModel.ConfigViewModel) // Return programm settings dialog instance
        win = new EdiViews.Config.ConfigDlg();
      else
      if (viewModel is EdiViews.About.AboutViewModel)             // Return about programm dialog instance
        win = new EdiViews.About.AboutDlg();
      else
      if (viewModel is EdiViews.GotoLine.GotoLineViewModel)       // Return goto line dialog instance
        win = new EdiViews.GotoLine.GotoLineDlg();
      else
      if (viewModel is EdiViews.FindReplace.ViewModel.FindReplaceViewModel) // Return find replace dialog instance
        win = new EdiViews.FindReplace.FindReplaceDialog();

      if (win != null)
      {
        win.Owner = parent;
        win.DataContext = viewModel;

        return win;
      }

      throw new NotSupportedException(viewModel.GetType().ToString());
    }
  }
}
