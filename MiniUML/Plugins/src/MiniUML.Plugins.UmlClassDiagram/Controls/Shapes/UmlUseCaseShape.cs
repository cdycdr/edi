namespace MiniUML.Plugins.UmlClassDiagram.Controls.Shapes
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using System.Xml.Linq;

  using MiniUML.Framework;
  using MiniUML.View.Controls;
  using MiniUML.View.Views;

  /// <summary>
  /// Interaction logic for UmlUseCaseShape.xaml
  /// </summary>
  public partial class UmlUseCaseShape : Control, ISnapTarget
  {
    #region constructor
    /// <summary>
    /// static class constructor
    /// </summary>
    static UmlUseCaseShape()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(UmlUseCaseShape),
      new FrameworkPropertyMetadata(typeof(UmlUseCaseShape)));
    }

    /// <summary>
    /// class constructor
    /// </summary>
    public UmlUseCaseShape()
    {
      this.ContextMenu = new ContextMenu();
      this.createContextMenu();
    }
    #endregion constructor

    public event SnapTargetUpdateHandler SnapTargetUpdate;

    #region methods
    public void SnapPoint(ref Point snapPosition, out double snapAngle)
    {
      snapAngle = 0;

      var cv = CanvasView.GetCanvasView(this);
      if (cv == null)
        return;

      Point pos = cv.ElementFromControl(this).GetPositionAttributes();

      // An array of line segments, each line segment represented as four double values.
      double[] shape = {
                pos.X, pos.Y,
                pos.X, pos.Y + ActualHeight,

                pos.X, pos.Y + ActualHeight,
                pos.X + ActualWidth, pos.Y + ActualHeight,

                pos.X + ActualWidth, pos.Y + ActualHeight,
                pos.X + ActualWidth, pos.Y,

                pos.X + ActualWidth, pos.Y,
                pos.X, pos.Y,
            };

      Point bestSnapPoint = new Point(double.NaN, double.NaN);
      double bestSnapLengthSq = double.PositiveInfinity;

      for (int i = 0; i < shape.Length; i += 4)
      {
        Point from = new Point(shape[i], shape[i + 1]);
        Point to = new Point(shape[i + 2], shape[i + 3]);

        AnchorPoint.SnapToLineSegment(from, to, snapPosition, ref bestSnapLengthSq, ref bestSnapPoint, ref snapAngle);
      }

      snapPosition = bestSnapPoint;
    }

    public void NotifySnapTargetUpdate(SnapTargetUpdateEventArgs e)
    {
      if (this.SnapTargetUpdate != null)
        this.SnapTargetUpdate(this, e);
    }

    /// <summary>
    /// Use mouse click event to acquire focus
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

    protected void AddMenuItem(string text, string id, MenuBase menu)
    {
      MenuItem menuItem = new MenuItem() { Header = text, Tag = id };
      menuItem.Click += this.menuItem_Click;
      menu.Items.Add(menuItem);
    }

    protected void AddZOrderMenuItems(MenuBase menu)
    {
      this.AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_BringToFront, "BringToFront", menu);
      this.AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_SendToBack, "SendToBack", menu);
    }

    protected void AddCopyCutPasteMenuItems(MenuBase menu)
    {
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Cut });
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Copy });
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Paste });
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Delete });
    }

    protected void AddUndoRedoMenuItems(MenuBase menu)
    {
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Undo });
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Redo });
    }

    protected void menuItem_Click(object sender, RoutedEventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;
      XElement element = DataContext as XElement;
      XElement root = element.Parent;

      switch ((string)menuItem.Tag)
      {
        case "BringToFront":
          element.Remove();
          root.Add(element);
          break;

        case "SendToBack":
          element.Remove();
          root.AddFirst(element);
          break;
      }
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);

      this.NotifySnapTargetUpdate(new SnapTargetUpdateEventArgs());
    }

    private void createContextMenu()
    {
      this.AddCopyCutPasteMenuItems(this.ContextMenu);

      this.ContextMenu.Items.Add(new Separator());
      this.AddUndoRedoMenuItems(this.ContextMenu);

      this.ContextMenu.Items.Add(new Separator());
      this.AddZOrderMenuItems(this.ContextMenu);
    }
    #endregion methods
  }
}
