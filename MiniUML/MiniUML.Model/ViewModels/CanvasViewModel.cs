﻿namespace MiniUML.Model.ViewModels
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
    #region constructor
    public CanvasViewModel(DocumentViewModel documentViewModel)
    {
      // Store a reference to the parent view model.
      this._DocumentViewModel = documentViewModel;

      // Create the commands in this view model.
      this._commandUtilities.InitializeCommands(this);
    }

    /***	
          CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, OnDelete(ApplicationCommands.NotACommand), CanDelete));
          AddBinding(EditingCommands.Delete, ModifierKeys.None, Key.Delete, OnDelete(EditingCommands.SelectRightByCharacter));

          AddBinding(EditingCommands.DeleteNextWord, ModifierKeys.Control, Key.Delete, OnDelete(EditingCommands.SelectRightByWord));
          AddBinding(EditingCommands.Backspace, ModifierKeys.None, Key.Back, OnDelete(EditingCommands.SelectLeftByCharacter));
          InputBindings.Add(TextAreaDefaultInputHandler.CreateFrozenKeyBinding(EditingCommands.Backspace, ModifierKeys.Shift, Key.Back)); // make Shift-Backspace do the same as plain backspace
          AddBinding(EditingCommands.DeletePreviousWord, ModifierKeys.Control, Key.Back, OnDelete(EditingCommands.SelectLeftByWord));
          AddBinding(EditingCommands.EnterParagraphBreak, ModifierKeys.None, Key.Enter, OnEnter);
          AddBinding(EditingCommands.EnterLineBreak, ModifierKeys.Shift, Key.Enter, OnEnter);
          AddBinding(EditingCommands.TabForward, ModifierKeys.None, Key.Tab, OnTab);
          AddBinding(EditingCommands.TabBackward, ModifierKeys.Shift, Key.Tab, OnShiftTab);
			
          CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, CanCutOrCopy));
          CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, OnCut, CanCutOrCopy));
          CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
			
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.DeleteLine, OnDeleteLine));
			
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.RemoveLeadingWhitespace, OnRemoveLeadingWhitespace));
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.RemoveTrailingWhitespace, OnRemoveTrailingWhitespace));
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertToUppercase, OnConvertToUpperCase));
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertToLowercase, OnConvertToLowerCase));
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertToTitleCase, OnConvertToTitleCase));
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.InvertCase, OnInvertCase));
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertTabsToSpaces, OnConvertTabsToSpaces));
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertSpacesToTabs, OnConvertSpacesToTabs));
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertLeadingTabsToSpaces, OnConvertLeadingTabsToSpaces));
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertLeadingSpacesToTabs, OnConvertLeadingSpacesToTabs));
          CommandBindings.Add(new CommandBinding(AvalonEditCommands.IndentSelection, OnIndentSelection));
			
          TextAreaDefaultInputHandler.WorkaroundWPFMemoryLeak(InputBindings);
     ***/

    #endregion constructor

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

    // Command properties
    public CommandModel cmd_Delete { get; private set; }
    public CommandModel cmd_Cut { get; private set; }
    public CommandModel cmd_Copy { get; private set; }
    public CommandModel cmd_Paste { get; private set; }
    public CommandModel cmd_Select { get; private set; }

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
        viewModel.cmd_Select = new SelectCommandModel(viewModel);
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
        this.Description = "Delete the selected shapes.";
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
        this.Description = "Cut the selected shapes to the clipboard.";
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
        this.Description = "Copy the selected shapes to the clipboard.";
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
        this.Description = "Paste the contents of the clipboard into the document.";
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
          Msg.Show("The clipboard does not contain any shapes!", "Unexpected Error", MsgBoxButtons.OK, MsgBoxImage.Warning);
        }
      }

      private CanvasViewModel _viewModel;
    }

    /// <summary>
    /// Private implementation of the Select command
    /// </summary>
    private class SelectCommandModel : CommandModel
    {
      public SelectCommandModel(CanvasViewModel viewModel)
        : base(ApplicationCommands.Stop)
      {
        _viewModel = viewModel;
        this.Name = "Select";
        this.Description = "Switch to select mode.";
        this.Image = new BitmapImage(new Uri("/MiniUML.Plugins.UmlClassDiagram;component/Resources/Images/Command.Select.png", UriKind.Relative));
      }

      public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
      {
        e.CanExecute = (_viewModel.CanvasViewMouseHandler != null);
        e.Handled = true;
      }

      public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
      {
        _viewModel.CancelCanvasViewMouseHandler();
      }

      private CanvasViewModel _viewModel;
    }

    #endregion
    #endregion
  }
}