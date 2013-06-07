namespace MiniUML.Plugins.UmlClassDiagram.Resources.Shapes
{
  using System.Windows.Controls;

  public class UmlClassShape : GenericUmlContainerShape
  {
    public UmlClassShape()
    {
      createContextMenu();
    }

    private void createContextMenu()
    {
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_ADD_METHOD, "AddMethod");
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_ADD_PROPERTY, "AddProperty");
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_ADD_FIELD, "AddField");
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_ADD_EVENT, "AddEvent");
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_ADD_CLASS, "AddClass");

      ContextMenu.Items.Add(new Separator());
      
      AddMenuItem(MiniUML.Framework.Local.Strings.STR_MENUITEM_DELETE, "Delete");
      AddZOrderMenuItems();
    }
  }
}