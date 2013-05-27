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
    #region constructor
    public PluginViewModel(IMiniUMLDocument windowViewModel)
    {
      // Store a reference to the parent view model.
      _WindowViewModel = windowViewModel;

      // Create the commands in this view model.
      _commandUtilities.InitializeCommands(this);
    }
    #endregion constructor

    #region View models

    public IMiniUMLDocument _WindowViewModel { get; private set; }

    #endregion

    #region Commands

    // Command properties
    public CommandModel cmd_CreateInterfaceShape { get; private set; }
    public CommandModel cmd_CreateAbstractClassShape { get; private set; }
    public CommandModel cmd_CreateClassShape { get; private set; }
    public CommandModel cmd_CreateStructShape { get; private set; }
    public CommandModel cmd_CreateEnumShape { get; private set; }

    // Connection types (arrows, triangles and such)
    public CommandModel cmd_CreateAssociationShape { get; private set; }
    public CommandModel cmd_CreateInheritanceShape { get; private set; }
    public CommandModel cmd_CreateAggregationShape { get; private set; }
    public CommandModel cmd_CreateCompositionShape { get; private set; }

    // Comment
    public CommandModel cmd_CreateCommentShape { get; private set; }

    #region Command implementations

    private CommandUtilities _commandUtilities = new CommandUtilities();
    #endregion

    #endregion
  }
}
