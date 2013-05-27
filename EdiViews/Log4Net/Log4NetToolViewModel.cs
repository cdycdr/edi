namespace EdiViews.Log4Net
{
  using System;
  using Edi.ViewModel;
  using Edi.ViewModel.Base;
  using EdiViews.Documents.Log4Net;

  /// <summary>
  /// This viewmodel manages the functions of the Log4Net Tool Window which contains
  /// filter function to adjust the display of log4net information.
  /// </summary>
  public class Log4NetToolViewModel : EdiViews.ViewModel.Base.ToolViewModel
  {
    #region fields
    public const string ToolContentId = "Log4NetTool";
    private Log4NetViewModel mLog4NetVM = null;
    #endregion fields

    #region constructor
    public Log4NetToolViewModel()
      : base("Log4Net")
    {
      // Check if active document is a log4net document to display data for...
      this.OnActiveDocumentChanged(null, null);

      ////Workspace.This.ActiveDocumentChanged += new EventHandler(OnActiveDocumentChanged);
      this.ContentId = ToolContentId;
    }
    #endregion constructor

    #region properties
    public override Uri IconSource
    {
      get
      {
        return new Uri("pack://application:,,,/EdiViews;component/Images/Documents/Log4net.png", UriKind.RelativeOrAbsolute);
      }
    }

    public Log4NetViewModel Log4NetVM
    {
      get
      {
        return this.mLog4NetVM;
      }

      protected set
      {
        if (this.mLog4NetVM != value)
        {
          this.mLog4NetVM = value;
          this.NotifyPropertyChanged(() => this.Log4NetVM);
          this.NotifyPropertyChanged(() => this.IsOnline);
        }
      }
    }

    public bool IsOnline
    {
      get
      {
        return (this.Log4NetVM != null);
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Executes event based when the active (AvalonDock) document changes.
    /// Determine whether tool window can show corresponding state or not
    /// and update viewmodel reference accordingly.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnActiveDocumentChanged(object sender, DocumentChangedEventArgs e)
    {
      if (e != null)
      {
        if (e.ActiveDocument != null)
        {
          Log4NetViewModel log4NetVM = e.ActiveDocument as Log4NetViewModel;

          if (log4NetVM != null)
            this.Log4NetVM = log4NetVM;  // There is an active Log4Net document -> display corresponding content
          else
            this.Log4NetVM = null;
        }
      }
      else // There is no active document hence we do not have corresponding content to display
      {
        this.Log4NetVM = null;
      }
    }
    #endregion methods
  }
}
