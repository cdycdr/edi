namespace MiniUML.Plugins.UmlClassDiagram.Resources.Shapes
{
  using System.Windows.Controls;

  public class UmlInterfaceShape : GenericUmlContainerShape
  {
    public UmlInterfaceShape()
    {
      createContextMenu();
    }

    private void createContextMenu()
    {
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_ADD_METHOD, "AddMethod");
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_ADD_PROPERTY, "AddProperty");
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_ADD_EVENT, "AddEvent");

      ContextMenu.Items.Add(new Separator());
      
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_DELETE, "Delete");

      AddZOrderMenuItems();
    }
  }
}
