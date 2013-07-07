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
  using System.ComponentModel;
  using System.Linq.Expressions;

  /// <summary>
  /// Interaction logic for UmlNodeShape.xaml
  /// </summary>
  public partial class UmlNodeShape : Control, ISnapTarget
  {
    private const double Border3DSize = 20;

    #region constructor
    /// <summary>
    /// static class constructor
    /// </summary>
    static UmlNodeShape()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(UmlNodeShape),
      new FrameworkPropertyMetadata(typeof(UmlNodeShape)));
    }

    /// <summary>
    /// class constructor
    /// </summary>
    public UmlNodeShape()
    {
      this.ContextMenu = new ContextMenu();
      this.createContextMenu();
    }
    #endregion constructor

    public event SnapTargetUpdateHandler SnapTargetUpdate;

    #region properties
    #region Second Square Points
    public Point P2_SecondSquare
    {
      get
      {
        return new Point((this.ActualWidth - 1 > 0 ? this.ActualWidth - 1 : this.ActualWidth), 1);
      }
    }

    public Point P3_SecondSquare
    {
      get
      {
        return new Point((this.ActualWidth - 1 > 0 ? this.ActualWidth - 1 : this.ActualWidth),
                         (this.ActualHeight - UmlNodeShape.Border3DSize > 0 ? this.ActualHeight - UmlNodeShape.Border3DSize : this.ActualHeight));
      }
    }
    #endregion Second Square Points

    #region First Square properties
    /**
     *     +--------+
     * P1/      P2 /|
     * +---------+  |
     * |         |  |
     * |         |  +
     * |         | /
     * +---------+/
     * P3       P4  FirstSquare Points
     ***/
    public Point P2_FirstSquare
    {
      get
      {
        return new Point((this.ActualWidth - UmlNodeShape.Border3DSize > 0 ? this.ActualWidth - UmlNodeShape.Border3DSize : this.ActualWidth),
                         (this.ActualHeight > UmlNodeShape.Border3DSize ? UmlNodeShape.Border3DSize : this.ActualHeight));
      }
    }

    public Point P3_FirstSquare
    {
      get
      {
        return new Point(1, (this.ActualHeight - 1 > 0 ? this.ActualHeight - 1 : this.ActualHeight));
      }
    }

    public Point P4_FirstSquare
    {
      get
      {
        return new Point((this.ActualWidth - UmlNodeShape.Border3DSize > 0 ? this.ActualWidth - UmlNodeShape.Border3DSize : this.ActualWidth),
                         (this.ActualHeight - 1 > 0 ? this.ActualHeight - 1 : this.ActualHeight));
      }
    }
    #endregion First Square properties
    #endregion properties

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
                pos.X, pos.Y + this.ActualHeight,

                pos.X, pos.Y + this.ActualHeight,
                pos.X + this.ActualWidth, pos.Y + this.ActualHeight,

                pos.X + this.ActualWidth, pos.Y + this.ActualHeight,
                pos.X + this.ActualWidth, pos.Y,

                pos.X + this.ActualWidth, pos.Y,
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
