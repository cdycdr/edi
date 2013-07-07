namespace MiniUML.Plugins.UmlClassDiagram.Controls.Shapes
{
  using System.Windows;
  using System.Windows.Controls;
  
  /// <summary>
  /// This class selects a canvas/path shape for a enumeration based parameter.
  /// </summary>
  public class UmlCanvasShapeSelector : DataTemplateSelector
  {
    public DataTemplate UmlManPathShape
    {
      get;
      set;
    }

    public DataTemplate ErrorPathShape
    {
      get;
      set;
    }

    public DataTemplate ActivityInitial
    {
      get;
      set;
    }

    public DataTemplate ActivityFinal
    {
      get;
      set;
    }

    public DataTemplate ActivityFlowFinal
    {
      get;
      set;
    }

    public DataTemplate ActivitySync
    {
      get;
      set;
    }

    public DataTemplate Event1
    {
      get;
      set;
    }

    public DataTemplate Event2
    {
      get;
      set;
    }

    /// <summary>
    /// Method is invoked with the Content property bound to a ContentControl
    /// if the <seealso cref="DataTemplateSelector"/> is assigned to the
    /// ContentTemplateSelector property of a ContentControl.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item is string)
      {
        string person = item as string;

        if (person == PathSelector.UmlMan.ToString())
          return this.UmlManPathShape;

        if (person == PathSelector.ActivityInitial.ToString())
          return this.ActivityInitial;

        if (person == PathSelector.ActivityFinal.ToString())
          return this.ActivityFinal;

        if (person == PathSelector.ActivityFlowFinal.ToString())
          return this.ActivityFlowFinal;

        if (person == PathSelector.ActivitySync.ToString())
          return this.ActivitySync;

        if (person == PathSelector.Event1.ToString())
          return this.Event1;

        if (person == PathSelector.Event2.ToString())
          return this.Event2;
      }

      return this.ErrorPathShape;
    }
  }
}
