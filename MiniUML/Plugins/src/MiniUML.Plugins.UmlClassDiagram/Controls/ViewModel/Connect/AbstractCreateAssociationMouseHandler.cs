namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Connect
{
  using System;
  using System.Windows;
  using System.Xml.Linq;

  using MiniUML.Framework;
  using MiniUML.Model.ViewModels;
  using MiniUML.View.Views;

  /// <summary>
  /// Class to handle mouseevents for drawing a line between two shapes.
  /// </summary>
  public abstract class AbstractCreateAssociationMouseHandler : ICanvasViewMouseHandler
  {
    #region fields
    private bool isDone = false;
    private bool hasBeenAdded = false;
    private XElement association;
    private AbstractDocumentViewModel viewModel;
    #endregion fields

    #region constructor
    /// <summary>
    /// Parameterized class constructor
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="association"></param>
    public AbstractCreateAssociationMouseHandler(AbstractDocumentViewModel viewModel, XElement association)
    {
      this.viewModel = viewModel;
      this.association = association;
      association.Add(new XElement("Anchor", new XAttribute("Left", "0"), new XAttribute("Top", "0")));
      association.Add(new XElement("Anchor", new XAttribute("Left", "0"), new XAttribute("Top", "0")));
    }
    #endregion constructor

    #region methods
    public void OnShapeClick(XElement shape)
    {
      if (this.isDone) return;

      this.viewModel.vm_CanvasViewModel.CancelCanvasViewMouseHandler();
    }

    public void OnShapeDragBegin(Point position, XElement shape)
    {
      if (this.isDone) return;

      if (shape == null || !this.IsValidFrom(shape))
      {
        this.viewModel.vm_CanvasViewModel.CancelCanvasViewMouseHandler();
        return;
      }

      string idString = shape.GetStringAttribute("Id");
      if (idString == string.Empty)
      {
        idString = this.viewModel.dm_DocumentDataModel.GetUniqueId();
        shape.SetAttributeValue("Id", idString);
      }

      this.association.SetAttributeValue("From", idString);
      ((XElement)this.association.FirstNode).SetPositionAttributes(position);

      // Add the shape to the document root.
      this.association = this.viewModel.dm_DocumentDataModel.AddShape(this.association);
      this.hasBeenAdded = true;

      ((CanvasView)this.viewModel.v_CanvasView).PresenterFromElement(this.association).IsHitTestVisible = false;
    }

    public void OnShapeDragUpdate(Point position, Vector delta)
    {
      if (this.isDone)
        return;

      ((XElement)this.association.LastNode).SetPositionAttributes(position);
    }

    public void OnShapeDragEnd(Point position, XElement shape)
    {
      if (this.isDone)
        return;

      if (shape == null || !this.IsValidTo(shape))
      {
        this.viewModel.vm_CanvasViewModel.CancelCanvasViewMouseHandler();
        return;
      }

      string idString = shape.GetStringAttribute("Id");
      if (idString == string.Empty)
      {
        idString = this.viewModel.dm_DocumentDataModel.GetUniqueId();
        shape.SetAttributeValue("Id", idString);
      }

      // HACK: Work around for odd not-quite-updated binding problem: Remove, then re-add.
      this.association.Remove();
      this.association.SetAttributeValue("To", idString);
      this.association = this.viewModel.dm_DocumentDataModel.AddShape(this.association);

      ((CanvasView)this.viewModel.v_CanvasView).PresenterFromElement(this.association).IsHitTestVisible = true;

      this.cleanUp();
      this.viewModel.vm_CanvasViewModel.FinishCanvasViewMouseHandler();
    }

    public void OnCancelMouseHandler()
    {
      if (this.hasBeenAdded)
        this.association.Remove();

      this.cleanUp();
    }

    protected abstract bool IsValidFrom(XElement element);

    protected abstract bool IsValidTo(XElement element);

    private void cleanUp()
    {
      this.viewModel.v_CanvasView.ForceCursor = true;
      this.viewModel.v_CanvasView.Cursor = null;
      this.isDone = true;
    }
    #endregion methods
  }
}
