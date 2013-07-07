namespace MiniUML.View.Views
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Xml.Linq;

  using MiniUML.Framework;
  using MiniUML.Model.ViewModels;
  using MiniUML.View.Controls;
  using MsgBox;
  using MiniUML.Framework.interfaces;
  using MiniUML.Framework.Command;

  public delegate void LayoutUpdatedHandler();

  /// <summary>
  /// Interaction logic for CanvasView.xaml
  /// </summary>
  public partial class CanvasView : UserControl, ICanvasViewMouseHandler
  {
    #region fields
    private bool _gotMouseDown = false;
    private ICanvasViewMouseHandler _currentMouseHandler = null;
    private Point _dragStart; // Mouse position at drag start / last drag update.
    private XElement _dragShape; // Shape under mouse at drag start.

    private HashSet<LayoutUpdatedHandler> _layoutUpdatedHandlers = new HashSet<LayoutUpdatedHandler>();

    #region Command Bindings
    static readonly List<CommandBinding> CmdBindings = new List<CommandBinding>();
    ////static readonly List<InputBinding> InputBindings = new List<InputBinding>();
    #endregion Command Bindings

    ItemsControl part_ItemsControl = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// class constructor
    /// </summary>
    public CanvasView()
    {
      // Copy static collection of commands to collection of commands of this instance
      this.CommandBindings.AddRange(CanvasView.CmdBindings);

      base.LayoutUpdated += canvas_LayoutUpdated;

      this.DataContextChanged += delegate(object sender, DependencyPropertyChangedEventArgs e)
      {
        if (_CanvasViewModel != null)
          _CanvasViewModel.SelectionChanged -= model_SelectionChanged;

        _CanvasViewModel = (CanvasViewModel)DataContext;

        if (_CanvasViewModel != null)
          _CanvasViewModel.SelectionChanged += model_SelectionChanged;
      };
    }

    /// <summary>
    /// static class constructor
    /// </summary>
    static CanvasView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CanvasView), new FrameworkPropertyMetadata(typeof(CanvasView)));      

      CmdBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, CanCopy));
      CmdBindings.Add(new CommandBinding(ApplicationCommands.Cut, OnCut, CanCut));
      CmdBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
      CmdBindings.Add(new CommandBinding(ApplicationCommands.Delete, OnDelete, CanDelete));
      CmdBindings.Add(new CommandBinding(ApplicationCommands.Undo, OnUndo, CanUndo));
      CmdBindings.Add(new CommandBinding(ApplicationCommands.Redo, OnRedo, CanRedo));
    }
    #endregion constructor

    #region properties
    public CanvasViewModel _CanvasViewModel { get; private set; }

    public static readonly DependencyProperty CustomDragProperty
      = DependencyProperty.RegisterAttached("CustomDrag", typeof(bool), typeof(CanvasView),
                                            new FrameworkPropertyMetadata(false));
    #endregion properties

    #region methods
    /// <summary>
    /// Standard method that is executed when a template is applied to a lookless control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      this.part_ItemsControl = this.GetTemplateChild("Part_ItemsControl") as ItemsControl;
    }

    /// <summary>
    /// Use mouse click event to get focus
    /// (ApplicationCommands.Copy, Cut, Paste will not work otherwise) 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);

      if (this.IsFocused == false)
        this.Focus();

      e.Handled = true;
    }

    #region Drag/Drop functionality

    /// <summary>
    /// Handles the Drop event of the canvas.
    /// When a IDragableCommand, which represents an item from a ribbon gallery, is dropped on the canvas, its OnDragDropExecute is called.
    /// </summary>
    protected override void OnDrop(DragEventArgs e)
    {
      base.OnDrop(e);

      if (this.part_ItemsControl == null)
        return;

      if (e.Data.GetDataPresent(typeof(IDragableCommandModel)))
      {
        IDragableCommandModel cmd = (IDragableCommandModel)e.Data.GetData(typeof(IDragableCommandModel));

        cmd.OnDragDropExecute(e.GetPosition(this.part_ItemsControl));
        e.Handled = true;
        return;
      }

      if (e.Data.GetDataPresent(typeof(MiniUML.Framework.helpers.DragObject)))
      {
        try
        {
          MiniUML.Framework.helpers.DragObject dragObject = (MiniUML.Framework.helpers.DragObject)e.Data.GetData(typeof(MiniUML.Framework.helpers.DragObject));

          IDragableCommandModel c = dragObject.ObjectInstance as IDragableCommandModel;

          if (c == null)
            return;

          Point p = e.GetPosition(this.part_ItemsControl);

          if (p != null)
            c.OnDragDropExecute(p);

          return;
        }
        catch (Exception ex)
        {
          Msg.Show(ex, "Drag & Drop operation aborted on error",
                       "An erro occurred", MsgBoxButtons.OK);
        }
      }

      string fileName = IsSingleFile(e);
      if (fileName != null)
      {
        //Check if the datamodel is ready
        if (!(_CanvasViewModel._DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
              _CanvasViewModel._DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid))
          return;

        Application.Current.MainWindow.Activate();
        if (!_CanvasViewModel._DocumentViewModel.QuerySaveChanges()) return;

        try
        {
          // Open the document.
          _CanvasViewModel._DocumentViewModel.LoadFile(fileName);
        }
        catch (Exception ex)
        {
          Msg.Show(ex, string.Format(MiniUML.Framework.Local.Strings.STR_OpenFILE_MSG, fileName),
                                     MiniUML.Framework.Local.Strings.STR_OpenFILE_MSG_CAPTION, MsgBoxButtons.OK);
        }
        return;
      }
    }

    /// <summary>
    /// Method is executed when the user drags an item over the canvas.
    /// The <seealso cref="DragDropEffects"/> enumeration is used to signal
    /// wheter drag & drop operation can continue or not (system shows a plus
    /// sign or stop sign on drag over)
    /// </summary>
    /// <param name="e"></param>
    protected override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);

      e.Effects = DragDropEffects.None;

      if (e.Data.GetDataPresent(typeof(IDragableCommandModel)))
      {
        e.Effects = DragDropEffects.Copy;
      }
      else
      {
        // the user dragged a toolbox item over wrapped in a DragObject over the canvas
        if (e.Data.GetDataPresent(typeof(MiniUML.Framework.helpers.DragObject)))
        {
          e.Effects = DragDropEffects.Copy;
        }
        else if (IsSingleFile(e) != null)
        {
          e.Effects = DragDropEffects.Copy;
        }
        else
          Console.WriteLine(e.Data.GetType().ToString());
      }

      e.Handled = true;
    }

    // Standard method to check the drag data; found in documentation.
    // If the data object in args is a single file, this method will return the filename.
    // Otherwise, it returns null.
    private string IsSingleFile(DragEventArgs args)
    {
      // Check for files in the hovering data object.
      if (args.Data.GetDataPresent(DataFormats.FileDrop, true))
      {
        string[] fileNames = args.Data.GetData(DataFormats.FileDrop, true) as string[];
        // Check fo a single file or folder.
        if (fileNames.Length == 1)
        {
          // Check for a file (a directory will return false).
          if (File.Exists(fileNames[0]))
          {
            // At this point we know there is a single file.
            return fileNames[0];
          }
        }
      }
      return null;
    }

    #endregion

    #region Select / move functionality
    public static void SetCustomDrag(UIElement element, bool value)
    {
      element.SetValue(CustomDragProperty, value);
    }

    public static bool GetCustomDrag(UIElement element)
    {
      return (bool)element.GetValue(CustomDragProperty);
    }

    /// <summary>
    /// Handles the PreviewMouseDown event of the canvas.
    /// If CanvasViewMouseHandler is set, we handle all mouse button events. Otherwise, only left-clicks.
    /// </summary>
    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      if (this.part_ItemsControl == null)
        return;

      if (_gotMouseDown || (_CanvasViewModel.CanvasViewMouseHandler == null && e.ChangedButton != MouseButton.Left))
      {
        base.OnPreviewMouseDown(e);
        return;
      }

      beginMouseOperation();

      _dragStart = e.GetPosition(this.part_ItemsControl);
      _dragShape = GetShapeAt(_dragStart);

      e.Handled = (_CanvasViewModel.CanvasViewMouseHandler != null) || (_CanvasViewModel.prop_SelectedShapes.Count > 1);
    }

    /// <summary>
    /// Handles the PreviewMouseUp event of the canvas.
    /// </summary>
    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      if (this.part_ItemsControl == null)
        return;

      if (!_gotMouseDown)
        return;

      Point position = e.GetPosition(this.part_ItemsControl);
      Vector dragDelta = position - _dragStart;

      if (!IsMouseCaptured)
      {
        if (e.LeftButton != MouseButtonState.Pressed) return; // We're not dragging anything.

        //
        if (IsMouseCaptureWithin) return; // This CanvasView is not responsible for the dragging.

        if (Math.Abs(dragDelta.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(dragDelta.Y) < SystemParameters.MinimumVerticalDragDistance) return;

        _currentMouseHandler.OnShapeDragBegin(position, _dragShape);

        CaptureMouse();
      }

      _currentMouseHandler.OnShapeDragUpdate(position, dragDelta);

      // The new "drag start" is the current mouse position.
      _dragStart = position;
    }

    protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
    {
      if (this.part_ItemsControl == null)
        return;

      if (!_gotMouseDown) return;

      try
      {
        Point position = e.GetPosition(part_ItemsControl);

        if (!IsMouseCaptured)
        {
          _currentMouseHandler.OnShapeClick(_dragShape);
          return;
        }

        ReleaseMouseCapture();

        _currentMouseHandler.OnShapeDragUpdate(position, position - _dragStart);
        _currentMouseHandler.OnShapeDragEnd(position, GetShapeAt(position));
      }
      finally
      {
        endMouseOperation();
      }

      /* HACK: Work-around for bug 4
      this._CanvasViewModel._DocumentViewModel.dm_DocumentDataModel.Undo();
      this._CanvasViewModel._DocumentViewModel.dm_DocumentDataModel.Redo();
       * */
    }

    private void beginMouseOperation()
    {
      //// DebugUtilities.Assert(_gotMouseDown == false, "beginMouseOperation called when already in mouse operation");
      _gotMouseDown = true;

      // Use the handler specified on Model, if not null. Otherwise, use ourself.
      _currentMouseHandler = _CanvasViewModel.CanvasViewMouseHandler != null ? _CanvasViewModel.CanvasViewMouseHandler : this;

      // Don't create undo states at every drag update.
      _CanvasViewModel._DocumentViewModel.dm_DocumentDataModel.BeginOperation("CanvasView mouse operation");
    }

    private void endMouseOperation()
    {
      _gotMouseDown = false;
      _currentMouseHandler = null;

      // Re-enable the data model.
      _CanvasViewModel._DocumentViewModel.dm_DocumentDataModel.EndOperation("CanvasView mouse operation");
    }

    /// <summary>
    /// Handles the SelectionChanged event of the view model.
    /// When the collection of selected shapes changes, the Selector.IsSelectedProperty is updated for all elements on the canvas.
    /// </summary>
    private void model_SelectionChanged(object sender, EventArgs e)
    {
      if (this.part_ItemsControl == null)
        return;

      foreach (XElement shape in this.part_ItemsControl.Items)
      {
        part_ItemsControl.ItemContainerGenerator.ContainerFromItem(shape).SetValue(
          Selector.IsSelectedProperty, _CanvasViewModel.prop_SelectedShapes.Contains(shape));
      }
    }
    #endregion

    #region ICanvasViewMouseHandler Members
    // Encapsulates functionality of shape selection by mouse (e.g. using ctrl to toggle selection).
    void ICanvasViewMouseHandler.OnShapeClick(XElement shape)
    {
      if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
      {
        if (shape == null) return;

        if (_CanvasViewModel.prop_SelectedShapes.Contains(shape))
          _CanvasViewModel.prop_SelectedShapes.Remove(shape);
        else _CanvasViewModel.prop_SelectedShapes.Add(shape);
      }
      else _CanvasViewModel.SelectShape(shape);
    }

    void ICanvasViewMouseHandler.OnShapeDragBegin(Point position, XElement shape)
    {
      if (!_CanvasViewModel.prop_SelectedShapes.Contains(_dragShape))
        ((ICanvasViewMouseHandler)this).OnShapeClick(_dragShape);

      /*
      for (int i = Model.prop_SelectedShapes.Count - 1; i >= 0; i--)
      {
          XElement e = Model.prop_SelectedShapes[i];
          if ((bool)ControlFromElement(e).GetValue(CanvasView.CustomDragProperty))
              Model.prop_SelectedShapes.RemoveAt(i);
      }
       */
    }

    void ICanvasViewMouseHandler.OnShapeDragUpdate(Point position, Vector delta)
    {
      if (this.part_ItemsControl == null)
        return;

      foreach (XElement shape in _CanvasViewModel.prop_SelectedShapes)
      {
        FrameworkElement control = ControlFromElement(shape);

        if ((bool)control.GetValue(CanvasView.CustomDragProperty)) continue;

        Point origin = shape.GetPositionAttributes();

        Point p = origin + delta;

        if (p.X < 0) p.X = 0;
        else
        {
          if (p.X + control.ActualWidth > part_ItemsControl.ActualWidth)
            p.X = this.part_ItemsControl.ActualWidth - control.ActualWidth;
        }

        if (p.Y < 0) p.Y = 0;
        else
        {
          if (p.Y + control.ActualHeight > part_ItemsControl.ActualHeight)
            p.Y = this.part_ItemsControl.ActualHeight - control.ActualHeight;
        }

        shape.SetPositionAttributes(p);

        ISnapTarget ist = control as ISnapTarget;
        if (ist != null)
          ist.NotifySnapTargetUpdate(new SnapTargetUpdateEventArgs(p - origin));
      }
    }

    void ICanvasViewMouseHandler.OnShapeDragEnd(Point position, XElement element)
    {
      CanvasViewModel viewModel = this.DataContext as CanvasViewModel;
    }

    void ICanvasViewMouseHandler.OnCancelMouseHandler()
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Coercion
    public void NotifyOnLayoutUpdated(LayoutUpdatedHandler handler)
    {
      _layoutUpdatedHandlers.Add(handler);
    }

    private void canvas_LayoutUpdated(object sender, EventArgs e)
    {
      if (_CanvasViewModel == null) return;

      HashSet<LayoutUpdatedHandler> set = _layoutUpdatedHandlers;
      if (set.Count == 0) return;
      _layoutUpdatedHandlers = new HashSet<LayoutUpdatedHandler>();

      try
      {
        _CanvasViewModel._DocumentViewModel.dm_DocumentDataModel.BeginOperation("CanvasView.canvas_LayoutUpdated");

        foreach (LayoutUpdatedHandler handler in set) handler();
      }
      finally
      {
        _CanvasViewModel._DocumentViewModel.dm_DocumentDataModel.EndOperationWithoutCreatingUndoState("CanvasView.canvas_LayoutUpdated");
      }
    }
    #endregion

    #region Utility methods

    public static CanvasView GetCanvasView(DependencyObject obj)
    {
      while (obj != null)
      {
        CanvasView cv = obj as CanvasView;
        if (cv != null) return cv;
        obj = VisualTreeHelper.GetParent(obj);
      }

      return null;
    }

    public XElement GetShapeAt(Point p)
    {
      if (this.part_ItemsControl == null)
        return null;

      DependencyObject hitObject = (DependencyObject)this.part_ItemsControl.InputHitTest(p);

      // Workaround: For reasons unknown, InputHitTest sometimes return null when it clearly should not. This appears to be a framework bug.
      if (hitObject == null) { return null; }

      // If hitObject is not a visual, we need to find the visual parent.
      // Thus we loop as long as we're dealing with a FrameworkContentElement.
      // Only FrameworkContentElements expose a Parent property, so we cast.
      // (If we find a generic ContentElement, something has gone horribly wrong.)
      while (hitObject is FrameworkContentElement)
        hitObject = ((FrameworkContentElement)hitObject).Parent;

      ContentPresenter presenter = null;

      do
      {
        if (hitObject is ContentPresenter)
          presenter = (ContentPresenter)hitObject;

        hitObject = VisualTreeHelper.GetParent(hitObject);
      }
      while (hitObject != this.part_ItemsControl);

      // Something's wrong: We clicked a control not wrapped in a ContentPresenter... Never mind, then.
      if (presenter == null)
        return null;

      var element = part_ItemsControl.ItemContainerGenerator.ItemFromContainer(presenter);
      
      return (element == DependencyProperty.UnsetValue) ? null : (XElement)element;
    }

    public UIElement PresenterFromElement(XElement element)
    {
      if (element == null)
        return null;

      if (this.part_ItemsControl == null)
        return null;
      
      return (UIElement)this.part_ItemsControl.ItemContainerGenerator.ContainerFromItem(element);
    }

    public FrameworkElement ControlFromElement(XElement element)
    {
      if (element == null)
        return null;

      if (this.part_ItemsControl == null)
        return null;

      DependencyObject dob = this.part_ItemsControl.ItemContainerGenerator.ContainerFromItem(element);

      if (dob == null)
        return null;

      // Fix for exception:
      // VisualTreeHelper.GetChild
      // Message: Specified index is out of range or child at index is null. Do not call this method if
      //          VisualChildrenCount returns zero, indicating that the Visual has no children.
      //          (System.ArgumentOutOfRangeException)
      if(VisualTreeHelper.GetChildrenCount(dob) == 0)
        return null;

      return (FrameworkElement)VisualTreeHelper.GetChild(dob, 0);
    }

    public XElement ElementFromControl(DependencyObject shape)
    {
      if (this.part_ItemsControl == null)
        return null;

      while (shape != null)
      {
        XElement item = this.part_ItemsControl.ItemContainerGenerator.ItemFromContainer(shape) as XElement;
        if (item != null) return item;
        shape = VisualTreeHelper.GetParent(shape);
      }

      return null;
    }

    #endregion

    #region Clipboard commands
    #region static methods
    static void CanCopy(object target, CanExecuteRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandCopy_CanExecute(target, args);
    }

    static void CanCut(object target, CanExecuteRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandCut_CanExecute(target, args);
    }

    static void CanPaste(object target, CanExecuteRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandPaste_CanExecute(target, args);
    }

    static void CanDelete(object target, CanExecuteRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandDelete_CanExecute(target, args);
    }

    static void CanUndo(object target, CanExecuteRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandUndo_CanExecute(target, args);
    }

    static void CanRedo(object target, CanExecuteRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandRedo_CanExecute(target, args);
    }

    static void OnCopy(object target, ExecutedRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandCopy_Executed(target, args);
      args.Handled = true;
    }

    static void OnCut(object target, ExecutedRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandCut_Executed(target, args);
      args.Handled = true;
    }

    static void OnPaste(object target, ExecutedRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandPaste_Executed(target, args);
      args.Handled = true;
    }

    static void OnUndo(object target, ExecutedRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandUndo_Executed(target, args);
      args.Handled = true;
    }

    static void OnRedo(object target, ExecutedRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandRedo_Executed(target, args);
      args.Handled = true;
    }

    static void OnDelete(object target, ExecutedRoutedEventArgs args)
    {
      CanvasView cv = target as CanvasView;

      if (cv == null)
        return;

      cv.CommandDelete_Executed(target, args);
      args.Handled = true;
    }
    #endregion static methods

    #region Copy Command
    void CommandCopy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;

      if (_CanvasViewModel != null)
      {
        if (_CanvasViewModel.cmd_Copy != null)
        {
          _CanvasViewModel.cmd_Copy.OnQueryEnabled(sender, e);
        }
      }
      e.Handled = true;
    }

    void CommandCopy_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (_CanvasViewModel != null)
        if (_CanvasViewModel.cmd_Copy != null)
        {
          _CanvasViewModel.cmd_Copy.OnExecute(sender, e);
        }

      e.Handled = true;
    }
    #endregion Copy Command

    #region Cut Command
    void CommandCut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;

      if (_CanvasViewModel != null)
      {
        if (_CanvasViewModel.cmd_Cut != null)
        {
          _CanvasViewModel.cmd_Cut.OnQueryEnabled(sender, e);
        }
      }
      e.Handled = true;
    }

    void CommandCut_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (_CanvasViewModel != null)
        if (_CanvasViewModel.cmd_Cut != null)
        {
          _CanvasViewModel.cmd_Cut.OnExecute(sender, e);
        }

      e.Handled = true;
    }
    #endregion Cut Command

    #region Paste Command
    void CommandPaste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;

      if (_CanvasViewModel != null)
      {
        if (_CanvasViewModel.cmd_Paste != null)
        {
          _CanvasViewModel.cmd_Paste.OnQueryEnabled(sender, e);
        }
      }
      e.Handled = true;
    }

    void CommandPaste_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (_CanvasViewModel != null)
        if (_CanvasViewModel.cmd_Paste != null)
        {
          _CanvasViewModel.cmd_Paste.OnExecute(sender, e);
        }

      e.Handled = true;
    }
    #endregion Paste Command

    #region Delete Command
    void CommandDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;

      if (_CanvasViewModel != null)
      {
        if (_CanvasViewModel.cmd_Delete != null)
        {
          _CanvasViewModel.cmd_Delete.OnQueryEnabled(sender, e);
        }
      }
      e.Handled = true;
    }

    void CommandDelete_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (_CanvasViewModel != null)
        if (_CanvasViewModel.cmd_Delete != null)
        {
          _CanvasViewModel.cmd_Delete.OnExecute(sender, e);
        }

      e.Handled = true;
    }
    #endregion Delete Command

    #region Undo Command
    public void CommandUndo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;

      if (_CanvasViewModel == null)
        return;

      if (_CanvasViewModel._DocumentViewModel == null)
        return;

      _CanvasViewModel._DocumentViewModel.cmd_Undo.OnQueryEnabled(sender, e);
      e.Handled = true;
    }

    public void CommandUndo_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (_CanvasViewModel == null)
        return;

      if (_CanvasViewModel._DocumentViewModel == null)
        return;

      _CanvasViewModel._DocumentViewModel.cmd_Undo.OnExecute(sender, e);
    }
    #endregion Undo Command

    #region Redo Command
    public void CommandRedo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;

      if (_CanvasViewModel == null)
        return;

      if (_CanvasViewModel._DocumentViewModel == null)
        return;

      _CanvasViewModel._DocumentViewModel.cmd_Redo.OnQueryEnabled(sender, e);
      e.Handled = true;
    }

    public void CommandRedo_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (_CanvasViewModel == null)
        return;

      if (_CanvasViewModel._DocumentViewModel == null)
        return;

      _CanvasViewModel._DocumentViewModel.cmd_Redo.OnExecute(sender, e);
    }
    #endregion Redo Command
    #endregion
    #endregion methods
  }
}
