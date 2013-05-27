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
    /// Private implementation of the CreateStructShape command.
    /// </summary>
    private class CreateStructShapeCommandModel : CommandModel, IDragableCommandModel
    {
      public CreateStructShapeCommandModel(PluginViewModel viewModel)
      {
        _viewModel = viewModel;
        this.Name = "Struct";
        this.Description = "Create a new Struct shape";
        this.Image = new BitmapImage(new Uri("/MiniUML.Plugins.UmlClassDiagram;component/Resources/Images/Command.CreateStructShape.png", UriKind.Relative));
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
        OnDragDropExecute(new Point(10, 10));
      }

      public void OnDragDropExecute(Point dropPoint)
      {
        AbstractDocumentViewModel documentViewModel = _viewModel._WindowViewModel.vm_DocumentViewModel;

        XElement element = new XElement("Uml.Struct",
                           new XAttribute("Name", "<name>"),
                           new XAttribute("Description", "Struct"),
                           new XAttribute("Top", dropPoint.Y),
                           new XAttribute("Left", dropPoint.X));

        documentViewModel.dm_DocumentDataModel.DocumentRoot.Add(element);
      }

      private PluginViewModel _viewModel;
    }
  }
}
