namespace MiniUML.Model.ViewModels
{
  /// <summary>
  /// Define an interface to reference to a MiniUML Document.
  /// This interface is used to reference MiniUML documents in any MiniUML PLugIn.
  /// </summary>
  public interface IMiniUMLDocument
  {
    MiniUML.Model.ViewModels.AbstractDocumentViewModel vm_DocumentViewModel { get; }
  }
}
