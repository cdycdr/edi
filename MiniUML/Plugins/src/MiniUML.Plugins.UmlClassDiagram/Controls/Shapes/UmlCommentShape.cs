namespace MiniUML.Plugins.UmlClassDiagram.Controls.Shapes
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using System.Xml.Linq;

  /// <summary>
  /// Interaction logic for UmlCommentShape.xaml
  /// </summary>
  public partial class UmlCommentShape : UserControl
  {
    #region constructor
    /// <summary>
    /// static class constructor
    /// </summary>
    static UmlCommentShape()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(UmlCommentShape),
      new FrameworkPropertyMetadata(typeof(UmlCommentShape)));
    }

    /// <summary>
    /// class constructor
    /// </summary>
    public UmlCommentShape()
    {
      this.ContextMenu = new ContextMenu();
      this.createContextMenu();
    }
    #endregion constructor

    #region methods
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
