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
    /// Private implementation of the CreateAssociationShape command.
    /// </summary>
    private class CreateCompositionShapeCommandModel : CommandModel
    {
      public CreateCompositionShapeCommandModel(PluginViewModel viewModel)
      {
        _viewModel = viewModel;
        this.Name = "Composition";
        this.Description = "Create a new composition";
        this.Image = new BitmapImage(new Uri("/MiniUML.Plugins.UmlClassDiagram;component/Resources/Images/Command.CreateCompositionShape.png", UriKind.Relative));
      }

      public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
      {
        if (_viewModel._WindowViewModel == null)
          return;

        if (_viewModel._WindowViewModel.vm_DocumentViewModel == null)
          return;

        e.CanExecute = _viewModel._WindowViewModel.vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready;
        e.Handled = true;
      }

      public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
      {
        var model = _viewModel._WindowViewModel.vm_DocumentViewModel;

        model.v_CanvasView.ForceCursor = true;
        model.v_CanvasView.Cursor = Cursors.Pen;

        model.vm_CanvasViewModel.BeginCanvasViewMouseHandler(
            new CreateAssociationMouseHandler(model, new XElement("Uml.Association.Composition")));
      }

      private PluginViewModel _viewModel;

      public class CreateAssociationMouseHandler : CommandUtilities.AbstractCreateAssociationMouseHandler
      {
        private XElement _fromElement;

        public CreateAssociationMouseHandler(AbstractDocumentViewModel viewModel, XElement association)
          : base(viewModel, association) { }

        protected override bool IsValidFrom(XElement element)
        {
          string[] validShapes = { "Uml.Interface", "Uml.Class", "Uml.AbstractClass", "Uml.Struct" };

          _fromElement = element;

          foreach (string s in validShapes)
            if (element.Name == s) return true;

          return false;
        }

        protected override bool IsValidTo(XElement element)
        {
          string[] validShapes = { "Uml.Interface", "Uml.Class", "Uml.AbstractClass", "Uml.Struct", "Uml.Enum" };

          foreach (string s in validShapes)
            if (element.Name == s) return true;

          return false;
        }
      }
    }
  }
}
