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

      if (viewModel is EdiViews.About.AboutViewModel)
      {
        EdiViews.About.AboutDlg win = new EdiViews.About.AboutDlg() { Owner = parent };

        ((EdiViews.About.AboutViewModel)viewModel).InitDialogInputData();
        win.DataContext = viewModel;

        return win;
      }

      if (viewModel is EdiViews.GotoLine.GotoLineViewModel)
        return new EdiViews.GotoLine.GotoLineDlg();

      if (viewModel is EdiViews.FindReplace.ViewModel.FindReplaceViewModel)
        return new EdiViews.FindReplace.FindReplaceDialog();

      throw new NotSupportedException(viewModel.GetType().ToString());
    }
  }
}
