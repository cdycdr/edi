namespace MiniUML.Model.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Media.Imaging;
  using System.Xml.Linq;
  using System.Windows.Documents;
  using System.Windows.Threading;

  using MiniUML.Framework;
  using MiniUML.Model.DataModels;
  using MsgBox;
  using MiniUML.Framework.Command;

  public interface ICanvasViewMouseHandler
  {
    void OnShapeClick(XElement shape);
    void OnShapeDragBegin(Point position, XElement shape);
    void OnShapeDragUpdate(Point position, Vector delta);
    void OnShapeDragEnd(Point position, XElement shape);

    void OnCancelMouseHandler();
  }

  public class CanvasViewModel : ViewModel
  {
    #region fields
    bool mIsFocused;
    private RelayCommand<object> mSelectCommand = null;
    #endregion fields

    #region constructor
    public CanvasViewModel(DocumentViewModel documentViewModel)
    {
      // Store a reference to the parent view model.
      this._DocumentViewModel = documentViewModel;

      // Create the commands in this view model.
      this._commandUtilities.InitializeCommands(this);

      this.mIsFocused = false;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// This property can be bound to a focus behaviour in a view
    /// to transfer focus when the viewmodel deems this to be appropriate.
    /// </summary>
    public bool IsFocused
    {
      get
      {
        return this.mIsFocused;
      }
      
      set
      {
        if (this.mIsFocused != value)
        {
          this.mIsFocused = value;
          this.SendPropertyChanged("IsFocused");
        }
      }
    }
    #endregion properties

    #region Mouse handling (CanvasViewMouseHandler)

    public ICanvasViewMouseHandler CanvasViewMouseHandler { get; private set; }

    public void BeginCanvasViewMouseHandler(ICanvasViewMouseHandler value)
    {
      if (CanvasViewMouseHandler == value) return; // no change

      if (CanvasViewMouseHandler != null) CancelCanvasViewMouseHandler();

      CanvasViewMouseHandler = value;
      if (CanvasViewMouseHandler != null)
      {
        _DocumentViewModel.dm_DocumentDataModel.BeginOperation("CanvasViewMouseHandler session");
      }
    }

    public void CancelCanvasViewMouseHandler()
    {
      var handler = CanvasViewMouseHandler;
      CanvasViewMouseHandler = null;
      try
      {
        handler.OnCancelMouseHandler();
      }
      finally
      {
        _DocumentViewModel.dm_DocumentDataModel.EndOperation("CanvasViewMouseHandler session");
      }
    }

    public void FinishCanvasViewMouseHandler()
    {
      CanvasViewMouseHandler = null;
      _DocumentViewModel.dm_DocumentDataModel.EndOperation("CanvasViewMouseHandler session");
    }

    #endregion

    #region Selection handling

    public event EventHandler SelectionChanged;

    public Collection<XElement> prop_SelectedShapes
    {
      get { return _selectedShapes; }
      set { SelectShapes(value); }
    }

    public void SelectShape(XElement shape)
    {
      _selectedShapes.CollectionChanged -= selectedShapes_CollectionChanged;

      _selectedShapes.Clear();

      if (shape != null)
        _selectedShapes.Add(shape);

      _selectedShapes.CollectionChanged += selectedShapes_CollectionChanged;
      selectedShapes_CollectionChanged(null, null);
    }

    public void SelectShapes(IEnumerable<XElement> shapes)
    {
      _selectedShapes.CollectionChanged -= selectedShapes_CollectionChanged;

      _selectedShapes.Clear();

      if (shapes != null)
        foreach (XElement shape in shapes) _selectedShapes.Add(shape);

      _selectedShapes.CollectionChanged += selectedShapes_CollectionChanged;
      selectedShapes_CollectionChanged(null, null);
    }

    private void selectedShapes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      SendPropertyChanged("prop_SelectedShapes");

      if (SelectionChanged != null)
        SelectionChanged(this, new EventArgs());

      CommandManager.InvalidateRequerySuggested();
    }

    private ObservableCollection<XElement> _selectedShapes = new ObservableCollection<XElement>();

    #endregion

    #region View models

    public DocumentViewModel _DocumentViewModel { get; private set; }

    #endregion

    #region Commands
    /// <summary>
    /// Get a command that can be used to active the mouse select shape mode.
    /// </summary>
    public RelayCommand<object> SelectCommand
    {
      get
      {
        if (this.mSelectCommand == null)
          this.mSelectCommand = new RelayCommand<object>((p) => this.OnSelectMode_Execute(),
                                                         (p) => this.OnSelectMode_CanExecute());
        return this.mSelectCommand;
      }
    }

    // Command properties
    public CommandModel cmd_Delete { get; private set; }
    public CommandModel cmd_Cut { get; private set; }
    public CommandModel cmd_Copy { get; private set; }
    public CommandModel cmd_Paste { get; private set; }

    private CommandUtilities _commandUtilities = new CommandUtilities();

    /// <summary>
    /// Utility class used by the command implementations.
    /// </summary>
    private class CommandUtilities
    {
      public void InitializeCommands(CanvasViewModel viewModel)
      {
        viewModel.cmd_Delete = new DeleteCommandModel(viewModel);
        viewModel.cmd_Cut = new CutCommandModel(viewModel);
        viewModel.cmd_Copy = new CopyCommandModel(viewModel);
        viewModel.cmd_Paste = new PasteCommandModel(viewModel);
      }
    }

    #region Command implementations

    /// <summary>
    /// Private implementation of the Delete command
    /// </summary>
    private class DeleteCommandModel : CommandModel
    {
      public DeleteCommandModel(CanvasViewModel viewModel)
        : base(ApplicationCommands.Delete)
      {
        _viewModel = viewModel;
        this.Name        = MiniUML.Framework.Local.Strings.STR_CMD_Delete;
        this.Description = MiniUML.Framework.Local.Strings.STR_CMD_Delete_description;

        this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Delete"];
      }

      public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
      {
        e.CanExecute = (_viewModel._DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready &&
                        _viewModel._selectedShapes.Count > 0);

        e.Handled = true;
      }

      public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
      {
        _viewModel._DocumentViewModel.dm_DocumentDataModel.BeginOperation("DeleteCommandModel.OnExecute");

        foreach (XElement shape in _viewModel._selectedShapes)
        {
          shape.Remove();
        }
        _viewModel._selectedShapes.Clear();

        _viewModel._DocumentViewModel.dm_DocumentDataModel.EndOperation("DeleteCommandModel.OnExecute");
      }

      private CanvasViewModel _viewModel;
    }

    /// <summary>
    /// Private implementation of the Cut command
    /// </summary>
    private class CutCommandModel : CommandModel
    {
      public CutCommandModel(CanvasViewModel viewModel)
        : base(ApplicationCommands.Cut)
      {
        _viewModel = viewModel;
        this.Description = MiniUML.Framework.Local.Strings.STR_CUT_Description;
        this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Cut"];
      }

      public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
      {
        e.CanExecute = (_viewModel._DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready &&
                        _viewModel._selectedShapes.Count > 0);

        e.Handled = true;
      }

      public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
      {
        _viewModel._DocumentViewModel.dm_DocumentDataModel.BeginOperation("CutCommandModel.OnExecute");

        XElement fragment = new XElement(DocumentDataModel.RootElementName);

        foreach (XElement shape in _viewModel._selectedShapes)
        {
          fragment.Add(shape);
          shape.Remove();
        }

        _viewModel._selectedShapes.Clear();
        Clipboard.SetText(fragment.ToString());

        _viewModel._DocumentViewModel.dm_DocumentDataModel.EndOperation("CutCommandModel.OnExecute");
      }

      private CanvasViewModel _viewModel;
    }

    /// <summary>
    /// Private implementation of the Copy command
    /// </summary>
    private class CopyCommandModel : CommandModel
    {
      public CopyCommandModel(CanvasViewModel viewModel)
        : base(ApplicationCommands.Copy)
      {
        _viewModel = viewModel;
        this.Description = MiniUML.Framework.Local.Strings.STR_COPY_Description;
        this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Copy"];
      }

      public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
      {
        e.CanExecute = (_viewModel._DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready &&
                        _viewModel._selectedShapes.Count > 0);

        e.Handled = true;
      }

      public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
      {
        XElement fragment = new XElement(DocumentDataModel.RootElementName);
        fragment.Add(_viewModel._selectedShapes);
        Clipboard.SetText(fragment.ToString());
      }

      private CanvasViewModel _viewModel;
    }

    /// <summary>
    /// Private implementation of the Paste command
    /// </summary>
    private class PasteCommandModel : CommandModel
    {
      public PasteCommandModel(CanvasViewModel viewModel)
        : base(ApplicationCommands.Paste)
      {
        _viewModel = viewModel;
        this.Description = MiniUML.Framework.Local.Strings.STR_PASTE_Description;
        this.Image = (BitmapImage)Application.Current.Resources["Style.Images.Commands.Paste"];
      }

      public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
      {
        e.CanExecute = _viewModel._DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready &&
                        Clipboard.ContainsText();

        e.Handled = true;
      }

      public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
      {
        try
        {
          XElement fragment = XElement.Parse(Clipboard.GetText());

          if (fragment.Name != DocumentDataModel.RootElementName)
            throw new Exception("Invalid root element.");

          _viewModel._DocumentViewModel.dm_DocumentDataModel.BeginOperation("PasteCommandModel.OnExecute");

          _viewModel._selectedShapes.Clear();
          foreach (XElement shape in fragment.Elements())
          {
            XElement copy = _viewModel._DocumentViewModel.dm_DocumentDataModel.AddShape(shape);
            _viewModel._selectedShapes.Add(copy);
          }

          _viewModel._DocumentViewModel.dm_DocumentDataModel.EndOperation("PasteCommandModel.OnExecute");
        }
        catch
        {
          Msg.Show(MiniUML.Framework.Local.Strings.STR_MSG_NoShapeInClipboard,
                   MiniUML.Framework.Local.Strings.STR_UnexpectedErrorCaption,
                   MsgBoxButtons.OK, MsgBoxImage.Warning);
        }
      }

      private CanvasViewModel _viewModel;
    }
    #endregion
    #endregion

    #region Selection mode command
    private bool OnSelectMode_CanExecute()
    {
      return (this.CanvasViewMouseHandler != null);
    }

    private void OnSelectMode_Execute()
    {
      this.CancelCanvasViewMouseHandler();
    }
    #endregion Selection mode command
  }
}
