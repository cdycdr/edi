namespace MiniUML.Model.ViewModels
{
  using MiniUML.Model.DataModels;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Windows;
  using System.Windows.Input;

  /// <summary>
  /// This document viewmodel class represents a minimal interface necessary to
  /// connect the <seealso cref="DocumentViewModel"/> with the outside world.
  /// </summary>
  public abstract class AbstractDocumentViewModel : MiniUML.Framework.ViewModel
  {
    #region properties
    /// <summary>
    /// The document data model contains the data that represents a diagram.
    /// </summary>
    public abstract DocumentDataModel dm_DocumentDataModel{ get; }

    /// <summary>
    /// The canvas viewmodel is the working area where the user is dragging stuff around
    /// </summary>
    public abstract FrameworkElement v_CanvasView{ get; set; }

    /// <summary>
    /// The canvas viewmodel is the data/state storage of the canvas working.
    /// </summary>
    public abstract CanvasViewModel vm_CanvasViewModel { get; }
    #endregion properties

    #region methods
    /// <summary>
    /// Export diagram into png or another imaging file type.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="defaultFileName"></param>
    /// <returns></returns>
    public abstract bool ExecuteExport(object sender, ExecutedRoutedEventArgs e, string defaultFileName);

    /// <summary>
    /// Load the contents of a file from the windows file system into memory and display it.
    /// </summary>
    /// <param name="filename"></param>
    public abstract void LoadFile(String filename);

    public abstract bool ExecuteSave(string filePath);
    #endregion methods
  }
}
