namespace FolderBrowser.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// Class a dialog viewmodel MVVM style...
  /// </summary>
  public class DialogViewModel : Base.ViewModelBase
  {
    #region fields
    private bool? mDialogCloseResult;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public DialogViewModel()
    {
      this.TreeBrowser = new BrowserViewModel();
      this.TreeBrowser.PathSelectionEvent += browser_PathSelectionEvent;
    }

    private void browser_PathSelectionEvent(object sender, Events.SelectedDirectoryEventArgs e)
    {
      this.DialogCloseResult = true;
    }
    #endregion constructor

    #region properties
    public BrowserViewModel TreeBrowser { get; set; }

    /// <summary>
    /// This can be used to close the attached view via ViewModel
    /// 
    /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
    /// </summary>
    public bool? DialogCloseResult
    {
      get
      {
        return this.mDialogCloseResult;
      }

      private set
      {
        if (this.mDialogCloseResult != value)
        {
          this.mDialogCloseResult = value;
          this.RaisePropertyChanged(() => this.DialogCloseResult);
        }
      }
    }
    #endregion properties

    #region methods
    #endregion methods
  }
}
