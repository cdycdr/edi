namespace MiniUML.Plugins.UmlClassDiagram.Resources.Shapes
{
  using System;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;
  using System.Windows.Shapes;
  using System.Xml.Linq;

  using MiniUML.Framework;
  using MiniUML.View.Controls;
  using MiniUML.View.Utilities;

  /// <summary>
  /// Interaction logic for Association.xaml
  /// </summary>
  public partial class GenericUmlAssociation : UserControl, INotifyPropertyChanged, ISnapTarget
  {
    #region Dependency properties (FromName, FromArrow, ToName, ToArrow)

    public static readonly DependencyProperty FromNameProperty = DependencyProperty.Register(
        "FromName",
        typeof(String),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender)
    );

    public String FromName
    {
      get { return (String)GetValue(FromNameProperty); }
      set { SetValue(FromNameProperty, value); }
    }

    public static readonly DependencyProperty ToNameProperty = DependencyProperty.Register(
        "ToName",
        typeof(String),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender)
    );

    public String ToName
    {
      get { return (String)GetValue(ToNameProperty); }
      set { SetValue(ToNameProperty, value); }
    }

    public static readonly DependencyProperty FromArrowProperty = DependencyProperty.Register(
        "FromArrow",
        typeof(String),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender)
    );

    public String FromArrow
    {
      get { return (String)GetValue(FromArrowProperty); }
      set { SetValue(FromArrowProperty, value); }
    }

    public static readonly DependencyProperty ToArrowProperty = DependencyProperty.Register(
        "ToArrow",
        typeof(String),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender)
    );

    public String ToArrow
    {
      get { return (String)GetValue(ToArrowProperty); }
      set { SetValue(ToArrowProperty, value); }
    }

    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
        "Stroke", typeof(Brush), typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender)
    );

    public Brush Stroke
    {
      get { return (Brush)GetValue(StrokeProperty); }
      set { SetValue(StrokeProperty, value); }
    }

    #endregion

    #region constructor
    public GenericUmlAssociation()
    {
      this.InitializeComponent();
      ((ShapeIdToControlConverter)Resources["ShapeIdToControlConverter"]).ReferenceControl = this;

      if (this.ContextMenu == null)
        this.ContextMenu = new ContextMenu();

      this.createContextMenu();
    }
    #endregion constructor

    #region INotifyPropertyChanged implementation

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion

    #region ISnapTarget Members

    public void SnapPoint(ref Point snapPosition, out double snapAngle)
    {
      tpl.SnapPoint(ref snapPosition, out snapAngle);
    }

    public event SnapTargetUpdateHandler SnapTargetUpdate;

    public void NotifySnapTargetUpdate(SnapTargetUpdateEventArgs e)
    {
      if (SnapTargetUpdate != null) SnapTargetUpdate(this, e);
    }

    private void snapTargetUpdate(ISnapTarget source, SnapTargetUpdateEventArgs e)
    {
      NotifySnapTargetUpdate(e);
    }

    public ShapeIdToControlConverter ShapeIdToControlConverter
    {
      get { return new ShapeIdToControlConverter() { ReferenceControl = this }; }
    }

    #endregion

    #region ContextMenu Methods
    private void createContextMenu()
    {
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_DELETE, "Delete");
      AddZOrderMenuItems();
    }

    protected void AddZOrderMenuItems()
    {
      if (this.ContextMenu != null)
        this.ContextMenu.Items.Add(new Separator());

      this.AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_BringToFront, "BringToFront");
      this.AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_SendToBack, "SendToBack");
    }

    protected void AddMenuItem(string text, string id)
    {
      MenuItem menuItem = new MenuItem() { Header = text, Tag = id };
      menuItem.Click += menuItem_Click;

      if (this.ContextMenu != null)
        this.ContextMenu.Items.Add(menuItem);
    }

    protected void menuItem_Click(object sender, RoutedEventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;
      XElement element = DataContext as XElement;
      XElement root = element.Parent;

      switch ((string)menuItem.Tag)
      {
        case "Delete":
          element.Remove();
          break;

        case "BringToFront":
          element.Remove();
          root.Add(element);
          //TODO: Add workaround for databinding bug.
          break;

        case "SendToBack":
          element.Remove();
          root.AddFirst(element);
          //TODO: Add workaround for databinding bug.
          break;
      }

      ListBox listBox = this.Template.FindName("_listBox", this) as ListBox;
      if (listBox != null) listBox.Focus();
    }
    #endregion ContextMenu Methods
  }

  /************************************************************************/

  public class ThreePieceLine : UserControl, INotifyPropertyChanged, ISnapTarget
  {
    #region fields
    private Polyline mLine;
    private Polyline mLineb;

    private double mFromAngleInDegrees;
    private double mToAngleInDegrees;
    #endregion fields

    #region dependency properties
    public static readonly DependencyProperty FromXProperty = DependencyProperty.Register(
        "FromX", typeof(Double), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged))
    );

    public Double FromX
    {
      get { return (Double)GetValue(FromXProperty); }
      set { SetValue(FromXProperty, value); }
    }

    public static readonly DependencyProperty FromYProperty = DependencyProperty.Register(
        "FromY", typeof(Double), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged))
    );

    public Double FromY
    {
      get { return (Double)GetValue(FromYProperty); }
      set { SetValue(FromYProperty, value); }
    }

    public static readonly DependencyProperty FromOrientationProperty = DependencyProperty.Register(
        "FromOrientation", typeof(Orientation), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged))
    );

    public Orientation FromOrientation
    {
      get { return (Orientation)GetValue(FromOrientationProperty); }
      set { SetValue(FromOrientationProperty, value); }
    }

    public static readonly DependencyProperty ToXProperty = DependencyProperty.Register(
        "ToX", typeof(Double), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged))
    );

    public Double ToX
    {
      get { return (Double)GetValue(ToXProperty); }
      set { SetValue(ToXProperty, value); }
    }

    public static readonly DependencyProperty ToYProperty = DependencyProperty.Register(
        "ToY", typeof(Double), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged))
    );

    public Double ToY
    {
      get { return (Double)GetValue(ToYProperty); }
      set { SetValue(ToYProperty, value); }
    }

    public static readonly DependencyProperty ToOrientationProperty = DependencyProperty.Register(
        "ToOrientation", typeof(Orientation), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged))
    );

    public Orientation ToOrientation
    {
      get { return (Orientation)GetValue(ToOrientationProperty); }
      set { SetValue(ToOrientationProperty, value); }
    }
    #endregion dependency properties

    #region constructor
    public ThreePieceLine()
    {
      this.mLine = new Polyline();

      this.mLineb = new Polyline()
      {
        StrokeThickness = 7,
        Stroke = new SolidColorBrush(new Color() { A = 192, R = 255, G = 255, B = 255 })
      };

      Canvas canvas = new Canvas();
      canvas.Children.Add(mLineb);
      canvas.Children.Add(mLine);
      this.Content = canvas;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get angle of the line joining the origin, in degrees.
    /// </summary>
    public double FromAngleInDegrees { get { return mFromAngleInDegrees; } }

    /// <summary>
    /// Get angle of the line joining the destination, in degrees.
    /// </summary>
    public double ToAngleInDegrees { get { return mToAngleInDegrees; } }
    #endregion properties

    #region methods
    private static void propChanged(DependencyObject o, DependencyPropertyChangedEventArgs a)
    {
      ((ThreePieceLine)o).rerouteLine();
    }

    private void rerouteLine()
    {
      Point from = new Point(FromX, FromY);
      Point to = new Point(ToX, ToY);

      mLine.Points = mLineb.Points;
      mLine.Points.Clear();
      mLine.Points.Add(from);

      if (FromOrientation != ToOrientation)
      {
        // Fine, we'll just do a two-piece line, then.
        if (FromOrientation == Orientation.Horizontal)
          mLine.Points.Add(new Point(to.X, from.Y));
        else mLine.Points.Add(new Point(from.X, to.Y));
      }
      else if (FromOrientation /* == ToOrientation */ == Orientation.Horizontal)
      {
        double mid = from.X + (to.X - from.X) / 2;

        mLine.Points.Add(new Point(mid, from.Y));
        mLine.Points.Add(new Point(mid, to.Y));
      }
      else /* FromOrientation == ToOrientation == Orientation.Vertical */
      {
        double mid = from.Y + (to.Y - from.Y) / 2;

        mLine.Points.Add(new Point(from.X, mid));
        mLine.Points.Add(new Point(to.X, mid));
      }

      mLine.Points.Add(to);

      Vector firstSegment = from - mLine.Points[1];
      Vector lastSegment = to - mLine.Points[mLine.Points.Count() - 2];
      mFromAngleInDegrees = FrameworkUtilities.RadiansToDegrees(firstSegment.GetAngularCoordinate());
      mToAngleInDegrees = FrameworkUtilities.RadiansToDegrees(lastSegment.GetAngularCoordinate());
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs("FromAngleInDegrees"));
        PropertyChanged(this, new PropertyChangedEventArgs("ToAngleInDegrees"));
      }
      NotifySnapTargetUpdate(new SnapTargetUpdateEventArgs());
    }

    #region ISnapTarget Members

    public void SnapPoint(ref Point snapPosition, out double snapAngle)
    {
      snapAngle = 0;

      PointCollection points = mLine.Points;
      if (points.Count < 2) return;

      Point bestSnapPoint = new Point(Double.NaN, Double.NaN);
      double bestSnapLengthSq = double.PositiveInfinity;

      Point lastPoint = points[0];
      for (int i = 1; i < points.Count; i++)
      {
        Point curPoint = points[i];
        AnchorPoint.SnapToLineSegment(lastPoint, curPoint, snapPosition, ref bestSnapLengthSq, ref bestSnapPoint, ref snapAngle);
        lastPoint = curPoint;
      }

      snapPosition = bestSnapPoint;
    }

    public event SnapTargetUpdateHandler SnapTargetUpdate;

    public void NotifySnapTargetUpdate(SnapTargetUpdateEventArgs e)
    {
      if (SnapTargetUpdate != null) SnapTargetUpdate(this, e);
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
    #endregion methods
  }
}
