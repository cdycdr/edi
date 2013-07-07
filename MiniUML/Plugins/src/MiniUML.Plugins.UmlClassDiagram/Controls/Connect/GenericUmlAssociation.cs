namespace MiniUML.Plugins.UmlClassDiagram.Controls.Connect
{
  using System;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Shapes;
  using System.Xml.Linq;

  using MiniUML.Framework;
  using MiniUML.View.Controls;
  using MiniUML.View.Converter;

  /// <summary>
  /// Interaction logic for Association.xaml
  /// </summary>
  public partial class GenericUmlAssociation : UserControl, INotifyPropertyChanged, ISnapTarget
  {
    #region fields
    private static readonly DependencyProperty FromNameProperty = DependencyProperty.Register(
        "FromName",
        typeof(string),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly DependencyProperty ToNameProperty = DependencyProperty.Register(
        "ToName",
        typeof(string),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly DependencyProperty FromArrowProperty = DependencyProperty.Register(
        "FromArrow",
        typeof(string),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly DependencyProperty ToArrowProperty = DependencyProperty.Register(
        "ToArrow",
        typeof(string),
        typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
        "Stroke", typeof(Brush), typeof(GenericUmlAssociation),
        new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    private ShapeIdToControlConverter shapeIdConverter = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// static class constructor
    /// </summary>
    static GenericUmlAssociation()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(GenericUmlAssociation),
      new FrameworkPropertyMetadata(typeof(GenericUmlAssociation)));
    }

    /// <summary>
    /// class constructor
    /// </summary>
    public GenericUmlAssociation()
    {
    }
    #endregion constructor

    public event SnapTargetUpdateHandler SnapTargetUpdate;

    /// <summary>
    /// Standard event of the <seealso cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #region Dependency properties (FromName, FromArrow, ToName, ToArrow)
    public string FromName
    {
      get { return (string)this.GetValue(FromNameProperty); }
      set { this.SetValue(FromNameProperty, value); }
    }

    public string ToName
    {
      get { return (string)this.GetValue(ToNameProperty); }
      set { this.SetValue(ToNameProperty, value); }
    }

    public string FromArrow
    {
      get { return (string)this.GetValue(FromArrowProperty); }
      set { this.SetValue(FromArrowProperty, value); }
    }

    public string ToArrow
    {
      get { return (string)this.GetValue(ToArrowProperty); }
      set { this.SetValue(ToArrowProperty, value); }
    }

    public Brush Stroke
    {
      get { return (Brush)this.GetValue(StrokeProperty); }
      set { this.SetValue(StrokeProperty, value); }
    }

    public ShapeIdToControlConverter ShapeIdToControlConverter
    {
      get
      {
        if (this.shapeIdConverter == null)
        {
          this.shapeIdConverter = new ShapeIdToControlConverter();
          this.shapeIdConverter.ReferenceControl = this;
        }

        return this.shapeIdConverter;
      }
    }
    #endregion

    #region methods

    public void SnapPoint(ref Point snapPosition, out double snapAngle)
    {
      snapAngle = 90;
      ////tpl.SnapPoint(ref snapPosition, out snapAngle);
    }

    public void NotifySnapTargetUpdate(SnapTargetUpdateEventArgs e)
    {
      if (this.SnapTargetUpdate != null)
        this.SnapTargetUpdate(this, e);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if (this.ContextMenu == null)
        this.ContextMenu = new ContextMenu();

      this.createContextMenu();
    }

    #region ContextMenu Methods
    protected void createContextMenu()
    {
      this.AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_DELETE, "Delete");
      this.AddZOrderMenuItems();
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
      menuItem.Click += this.menuItem_Click;

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
          root.Add(element); // TODO: Add workaround for databinding bug.
          break;

        case "SendToBack":
          element.Remove();
          root.AddFirst(element); // TODO: Add workaround for databinding bug.
          break;
      }

      ListBox listBox = this.Template.FindName("_listBox", this) as ListBox;
      if (listBox != null) listBox.Focus();
    }
    #endregion ContextMenu Methods

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);

      try
      {
        ShapeIdToControlConverter s = ((ShapeIdToControlConverter)this.FindResource("ShapeIdToControlConverter"));

        if (s != null)
          s.ReferenceControl = this;
      }
      catch (Exception)
      {
        Console.WriteLine("Exception: ShapeIdToControlConverter not found - target snapping may not work.");
      }
    }

    #region INotifyPropertyChanged implementation

    private void NotifyPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion

    private void snapTargetUpdate(ISnapTarget source, SnapTargetUpdateEventArgs e)
    {
      this.NotifySnapTargetUpdate(e);
    }
    #endregion
  }
}
