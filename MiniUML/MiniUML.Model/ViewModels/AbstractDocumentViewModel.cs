namespace MiniUML.Model.ViewModels
{
  using MiniUML.Model.DataModels;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Windows;

  public abstract class AbstractDocumentViewModel : MiniUML.Framework.ViewModel
  {
    public abstract DocumentDataModel dm_DocumentDataModel{ get; }

    public abstract FrameworkElement v_CanvasView{ get; set; }

    public abstract CanvasViewModel vm_CanvasViewModel { get; }
  }
}
