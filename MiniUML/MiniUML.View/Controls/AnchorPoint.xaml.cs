namespace MiniUML.View.Controls
{
  using System;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using MiniUML.Framework;
  using MiniUML.View.Views;

  /// <summary>
  /// Interaction logic for AnchorPoint.xaml
  /// </summary>
  public partial class AnchorPoint : Thumb, INotifyPropertyChanged
  {
    #region fields
    public static readonly ISnapTarget InvalidSnapTarget = new InvalidSnapTargetClass();
    private bool _inMoveTo = false;
    #endregion fields

    #region constructor
    public AnchorPoint()
    {
      this.InitializeComponent();
      base.DragDelta += this.AnchorPoint_DragDelta;
    }
    #endregion constructor

    #region properties
    public static readonly DependencyProperty SnapTargetProperty = DependencyProperty.Register(
        "SnapTarget",
        typeof(ISnapTarget),
        typeof(AnchorPoint),
        new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.AffectsRender,
            new PropertyChangedCallback(propChanged)
        )
    );

    public ISnapTarget SnapTarget
    {
      get { return (ISnapTarget)GetValue(SnapTargetProperty); }
      set { SetValue(SnapTargetProperty, value); }
    }

    public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
        "Left",
        typeof(double),
        typeof(AnchorPoint),
        new FrameworkPropertyMetadata(0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            new PropertyChangedCallback(propChanged)
        )
    );

    public double Left
    {
      get { return (double)GetValue(LeftProperty); }
      set { SetValue(LeftProperty, value); }
    }

    public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
        "Top",
        typeof(double),
        typeof(AnchorPoint),
        new FrameworkPropertyMetadata(0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            new PropertyChangedCallback(propChanged)
        )
    );

    public double Top
    {
      get { return (double)GetValue(TopProperty); }
      set { SetValue(TopProperty, value); }
    }

    /// <summary>
    /// The snap angle (argument of normal vector), normalized to 2π > angle ≥ 0.
    /// </summary>
    public double Angle { get; private set; }

    public double AngleInDegrees { get { return FrameworkUtilities.RadiansToDegrees(Angle); } }

    public Orientation SnapOrientation
    {
      get
      {
        double a = Angle;
        if (a > Math.PI) a = 2 * Math.PI - a;
        return a > Math.PI / 4 && a < Math.PI * 3 / 4 ? Orientation.Vertical : Orientation.Horizontal;
      }
    }
    #endregion properties

    #region methods
    public void MoveTo(Point position)
    {
      if (SnapTarget != null)
      {
        double angle;
        SnapTarget.SnapPoint(ref position, out angle);

        setAngle(angle);
      }

      _inMoveTo = true;
      try
      {
        Left = position.X;
        Top = position.Y;
      }
      finally
      {
        _inMoveTo = false;
      }
    }

    /// <summary>
    /// Static utility method for determining if and where to snap
    /// a point to a line segment (defined by the points from and to).
    /// </summary>
    public static bool SnapToLineSegment(Point from, Point to, Point snapOrigin, ref double bestSnapLengthSq, ref Point bestSnapPoint, ref double bestSnapAngle)
    {
      if (from == to) return false;

      Vector pointV = snapOrigin - from;
      Vector lineV = to - from;
      double resolute = pointV.ScalarProjection(lineV);

      // Limit resolute to the line segment between point1 and point2.
      if (resolute < 0) resolute = 0;
      else if (resolute > lineV.Length) resolute = lineV.Length;

      Point snapPoint = from + resolute * lineV / lineV.Length;
      double snapLengthSq = (snapPoint - snapOrigin).LengthSquared;

      if (snapLengthSq < bestSnapLengthSq)
      {
        bestSnapLengthSq = snapLengthSq;
        bestSnapPoint = snapPoint;
        bestSnapAngle = (to - from).GetAngularCoordinate() + Math.PI / 2;
        return true;
      }
      return false;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private static void propChanged(DependencyObject o, DependencyPropertyChangedEventArgs a)
    {
      AnchorPoint anchor = (AnchorPoint)o;
      if (a.Property == SnapTargetProperty)
      {
        if (a.OldValue != null) ((ISnapTarget)a.OldValue).SnapTargetUpdate -= anchor.snapTargetUpdate;

        if (a.NewValue == InvalidSnapTarget) anchor.SetValue(SnapTargetProperty, null);
        else if (a.NewValue != null) ((ISnapTarget)a.NewValue).SnapTargetUpdate += anchor.snapTargetUpdate;
      }
      else // Left or Top.
      {
        // If we're in a moveTo operation, coordinates have already been coerced. Otherwise, coerce them on the next layout update.
        if (!anchor._inMoveTo) anchor.CoerceLater();
      }
    }

    private void snapTargetUpdate(ISnapTarget source, SnapTargetUpdateEventArgs e)
    {
      if (e.isMoveUpdate) MoveTo(new Point(Left, Top) + e.moveDelta);
      else CoerceLater();
    }

    private void setAngle(double a)
    {
      a = FrameworkUtilities.NormalizeAngle(a);

      if (Angle == a) return; // no change
      Angle = a;
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs("SnapOrientation"));
        PropertyChanged(this, new PropertyChangedEventArgs("Angle"));
        PropertyChanged(this, new PropertyChangedEventArgs("AngleInDegrees"));
      }
    }

    #region Coercion
    private void CoerceLater()
    {
      CanvasView cv = CanvasView.GetCanvasView(this);
      
      if (cv != null)
        cv.NotifyOnLayoutUpdated(this.CoerceCoordinates);
    }

    public void CoerceCoordinates()
    {
      MoveTo(new Point(Left, Top));
    }
    #endregion

    private void AnchorPoint_DragDelta(object sender, DragDeltaEventArgs e)
    {
      MoveTo(new Point(Left + e.HorizontalChange, Top + e.VerticalChange));
    }
    #endregion methods

    #region private class
    /// <summary>
    /// This is an ugly HACK.
    /// </summary>
    private class InvalidSnapTargetClass : ISnapTarget
    {
      #region ISnapTarget Members
      /// <summary>
      /// Standard event of <seealso cref="ISnapTarget"/> interface.
      /// </summary>
      public event SnapTargetUpdateHandler SnapTargetUpdate;

      /// <summary>
      /// Standard method of <seealso cref="ISnapTarget"/> interface
      /// is not implemented and throws a <seealso cref="NotImplementedException"/> Exception.
      /// </summary>
      /// <param name="p"></param>
      /// <param name="snapAngle"></param>
      public void SnapPoint(ref Point p, out double snapAngle)
      {
        throw new NotImplementedException();
      }

      /// <summary>
      /// Standard method of <seealso cref="ISnapTarget"/> interface
      /// is not implemented and throws a <seealso cref="NotImplementedException"/> Exception.
      /// </summary>
      /// <param name="e"></param>
      public void NotifySnapTargetUpdate(SnapTargetUpdateEventArgs e)
      {
        throw new NotImplementedException();
      }
      #endregion
    }
    #endregion private class
  }
}
