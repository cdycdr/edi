namespace Edi.ViewModel
{
  using System;
  using System.Windows.Input;
  using SimpleControls.Command;
  using SimpleControls.MRU.ViewModel;

  public class StartPageViewModel : EdiViews.ViewModel.Base.FileBaseViewModel
  {
    public const string StartPageContentId = "{StartPage}";

    public StartPageViewModel()
    {
      this.Title = "Start Page";
      this.StartPageTip = "Welcome to Edi. Review this page to get started with this editor application.";
      this.ContentId = StartPageViewModel.StartPageContentId;
    }

    #region CloseCommand
    RelayCommand<object> _closeCommand = null;
    public override ICommand CloseCommand
    {
      get
      {
        if (_closeCommand == null)
        {
          _closeCommand = new RelayCommand<object>((p) => OnClose(), (p) => CanClose());
        }

        return _closeCommand;
      }
    }

    override public bool CanClose()
    {
      return true;
    }

    private void OnClose()
    {
      Workspace.This.Close(this);
    }
    #endregion

    override public bool CanSave() { return false; }

    override public bool CanSaveAs() { return false; }
    override public bool OnSaveAs() { return false; }
    override public string GetFilePath()
    {
      throw new NotSupportedException("Start Page does not have a valid file path.");
    }

    public override Uri IconSource
    {
      get
      {
        // This icon is visible in AvalonDock's Document Navigator window
        return new Uri("pack://application:,,,/Edi;component/Images/document.png", UriKind.RelativeOrAbsolute);
      }
    }

    public MRUListVM MruList
    {
      get
      {
        return Workspace.This.RecentFiles.MruList;
      }
    }

    public string  StartPageTip { get; set; }

    override public bool IsDirty
    {
      get
      {
        return false;
      }
      
      set
      {
        throw new NotSupportedException("Start page cannot be saved therfore setting dirty cannot be useful.");
      }
    }

    override public string FilePath
    {
      get
      {
        return this.ContentId;
      }
      
      protected set
      {
        throw new NotSupportedException();
      }
    }
  }
}
