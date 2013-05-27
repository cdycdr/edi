namespace MiniUML.Plugins.UmlClassDiagram
{
  using System;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Media.Imaging;
  using System.Xml.Linq;
  using MiniUML.Framework;
  using MiniUML.Model.ViewModels;
  using MiniUML.View.Views;

  public partial class PluginViewModel : ViewModel
  {
    /// <summary>
    /// Utility class used by the command implementations.
    /// </summary>
    private class CommandUtilities
    {
      /// <summary>
      /// Initialize commands such that they can produce user interaction when being executed.
      /// </summary>
      /// <param name="viewModel"></param>
      public void InitializeCommands(PluginViewModel viewModel)
      {
        viewModel.cmd_CreateInterfaceShape = new CreateInterfaceShapeCommandModel(viewModel);
        viewModel.cmd_CreateAbstractClassShape = new CreateAbstractClassShapeCommandModel(viewModel);
        viewModel.cmd_CreateClassShape = new CreateClassShapeCommandModel(viewModel);
        viewModel.cmd_CreateStructShape = new CreateStructShapeCommandModel(viewModel);
        viewModel.cmd_CreateEnumShape = new CreateEnumShapeCommandModel(viewModel);

        viewModel.cmd_CreateAssociationShape = new CreateAssociationShapeCommandModel(viewModel);
        viewModel.cmd_CreateInheritanceShape = new CreateInheritanceShapeCommandModel(viewModel);
        viewModel.cmd_CreateAggregationShape = new CreateAggregationShapeCommandModel(viewModel);
        viewModel.cmd_CreateCompositionShape = new CreateCompositionShapeCommandModel(viewModel);

        viewModel.cmd_CreateCommentShape = new CreateCommentShapeCommandModel(viewModel);
      }

      public abstract class AbstractCreateAssociationMouseHandler : ICanvasViewMouseHandler
      {
        private bool isDone = false;
        private Boolean hasBeenAdded = false;
        private XElement association;
        private AbstractDocumentViewModel viewModel;

        public AbstractCreateAssociationMouseHandler(AbstractDocumentViewModel viewModel, XElement association)
        {
          this.viewModel = viewModel;
          this.association = association;
          association.Add(new XElement("Anchor", new XAttribute("Left", "0"), new XAttribute("Top", "0")));
          association.Add(new XElement("Anchor", new XAttribute("Left", "0"), new XAttribute("Top", "0")));
        }

        public void OnShapeClick(XElement shape)
        {
          if (isDone) return;

          viewModel.vm_CanvasViewModel.CancelCanvasViewMouseHandler();
        }

        public void OnShapeDragBegin(Point position, XElement shape)
        {
          if (isDone) return;

          if (shape == null || !IsValidFrom(shape))
          {
            viewModel.vm_CanvasViewModel.CancelCanvasViewMouseHandler();
            return;
          }

          String idString = shape.GetStringAttribute("Id");
          if (idString == "")
          {
            idString = viewModel.dm_DocumentDataModel.GetUniqueId();
            shape.SetAttributeValue("Id", idString);
          }

          association.SetAttributeValue("From", idString);
          ((XElement)association.FirstNode).SetPositionAttributes(position);

          // Add the shape to the document root.
          association = viewModel.dm_DocumentDataModel.AddShape(association);
          hasBeenAdded = true;

          ((CanvasView)viewModel.v_CanvasView).PresenterFromElement(association).IsHitTestVisible = false;
        }

        public void OnShapeDragUpdate(Point position, Vector delta)
        {
          if (isDone) return;

          ((XElement)association.LastNode).SetPositionAttributes(position);
        }

        public void OnShapeDragEnd(Point position, XElement shape)
        {
          if (isDone) return;

          if (shape == null || !IsValidTo(shape))
          {
            viewModel.vm_CanvasViewModel.CancelCanvasViewMouseHandler();
            return;
          }

          string idString = shape.GetStringAttribute("Id");
          if (idString == "")
          {
            idString = viewModel.dm_DocumentDataModel.GetUniqueId();
            shape.SetAttributeValue("Id", idString);
          }

          // HACK: Work around for odd not-quite-updated binding problem: Remove, then re-add.
          association.Remove();
          association.SetAttributeValue("To", idString);
          association = viewModel.dm_DocumentDataModel.AddShape(association);

          ((CanvasView)viewModel.v_CanvasView).PresenterFromElement(association).IsHitTestVisible = true;

          cleanUp();
          viewModel.vm_CanvasViewModel.FinishCanvasViewMouseHandler();
        }

        public void OnCancelMouseHandler()
        {
          if (hasBeenAdded) association.Remove();
          cleanUp();
        }

        private void cleanUp()
        {
          viewModel.v_CanvasView.ForceCursor = true;
          viewModel.v_CanvasView.Cursor = null;
          isDone = true;
        }

        protected abstract bool IsValidFrom(XElement element);

        protected abstract bool IsValidTo(XElement element);
      }
    }
  }
}
